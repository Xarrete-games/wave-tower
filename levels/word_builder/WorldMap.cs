using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class WorldMap : Node2D
{
    private static readonly Vector2 PortalOffset = new(0, -80);
    private const int WavesPerBoss = 10;

    [Export]
    public Node2D visual;

    [Export]
    public CompositeTileMap composite_tile_map;

    [Export]
    public bool enable_fork = true;

    public Godot.Collections.Array<Godot.Collections.Dictionary> portal_entries { get; private set; } = new();
    public Godot.Collections.Array<Godot.Collections.Dictionary> finalized_portal_entries { get; private set; } = new();
    public Godot.Collections.Array<Vector2> portal_spawn_positions { get; private set; } = new();

    private Godot.Collections.Array<Variant> _mapPieces = new();
    private GodotObject _lastPieceAttached;
    private bool _hasPlacedFirstExpansion;
    private bool _pendingForkAfterBoss;

    private GodotObject _gridManager;
    private GodotObject _connectionGraph;
    private GodotObject _frontierManager;
    private GodotObject _routeBuilder;
    private GodotObject _spawnHandler;

    private RunProgress _progress;
    private Callable _edgeFinalizedCallable;

    public override void _Ready()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        this._mapPieces = dataLoader.get_all_map_pieces();

        Script gridManagerScript = GD.Load<Script>("res://levels/word_builder/grid_manager.gd");
        Script connectionGraphScript = GD.Load<Script>("res://levels/word_builder/piece_connection_graph.gd");
        Script frontierManagerScript = GD.Load<Script>("res://levels/word_builder/frontier_manager.gd");
        Script routeBuilderScript = GD.Load<Script>("res://levels/word_builder/route_builder.gd");
        Script spawnPositionsHandlerScript = GD.Load<Script>("res://levels/word_builder/spawn_positions_handler.gd");

        this._gridManager = gridManagerScript.Call("new").AsGodotObject();
        this._connectionGraph = connectionGraphScript.Call("new").AsGodotObject();
        this._frontierManager = frontierManagerScript.Call("new", this._gridManager, this._mapPieces).AsGodotObject();
        this._spawnHandler = spawnPositionsHandlerScript.Call("new", this.visual).AsGodotObject();

        if (this._gridManager == null || this._connectionGraph == null || this._frontierManager == null || this._spawnHandler == null)
        {
            GD.PushError("[WorldMap] Failed to initialize world builder managers.");
            return;
        }

        this._edgeFinalizedCallable = Callable.From<Variant, Variant>(this._on_edge_finalized);
        this._frontierManager.Connect("edge_finalized", this._edgeFinalizedCallable);

        GodotObject initPieceData = this._pick_random(this._safe_array(dataLoader.get_all_initial_map_pieces())).AsGodotObject();
        GodotObject initPiece = initPieceData?.Call("get_instance").AsGodotObject();
        if (initPiece == null)
        {
            GD.PushError("[WorldMap] Could not instantiate initial piece.");
            return;
        }

        AddChild(initPiece as Node);
        initPiece.Set("logical_pos", Vector2I.Zero);
        this._move_piece_decoration_to_visuals(initPiece);

        this._gridManager.Call("occupy", Vector2I.Zero);
        this._connectionGraph.Call("register_piece", initPiece);
        this.composite_tile_map?.register_piece(initPiece);
        this._frontierManager.Call("add_frontier", initPiece);

        this._routeBuilder = routeBuilderScript.Call("new", this._connectionGraph, initPiece).AsGodotObject();

        this._lastPieceAttached = initPiece;
        this.update_portals();
        this.attach_next_piece();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext.progress;
        this._progress.current_wave_finished += this._on_wave_finished;
    }

    public override void _ExitTree()
    {
        if (this._frontierManager != null && !this._edgeFinalizedCallable.Equals(default(Callable)) && this._frontierManager.IsConnected("edge_finalized", this._edgeFinalizedCallable))
        {
            this._frontierManager.Disconnect("edge_finalized", this._edgeFinalizedCallable);
        }

        if (this._progress != null)
        {
            this._progress.current_wave_finished -= this._on_wave_finished;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test"))
        {
            this.attach_next_piece();
        }
    }

    public void attach_next_piece()
    {
        if (!(bool)this._frontierManager.Call("has_frontiers"))
        {
            GD.PushError("No frontiers available for placement");
            return;
        }

        GodotObject frontier = this._frontierManager.Call("select_random_frontier").AsGodotObject();
        if (frontier == null)
        {
            GD.PushError("Failed to select frontier");
            return;
        }

        Godot.Collections.Array frontierEdges = this._safe_array(frontier.Get("edges"));
        if (frontierEdges.Count == 0)
        {
            this._frontierManager.Call("remove_frontier", frontier);
            this.update_portals();
            return;
        }

        Variant nextEdge = this._frontier_manager_pick_random_edge(frontier);
        Vector2I frontierLogicalPos = frontier.Get("logical_pos").AsVector2I();
        int nextEdgeDir = nextEdge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
        Vector2I candidateTile = this._gridManager.Call("get_neighbor_tile", frontierLogicalPos, nextEdgeDir).AsVector2I();

        Godot.Collections.Dictionary validation = this._frontierManager.Call("validate_edge", frontier, nextEdge, candidateTile).AsGodotDictionary();
        if (!validation.ContainsKey("valid") || !validation["valid"].AsBool())
        {
            string reason = validation.ContainsKey("reason") ? validation["reason"].AsString() : "unknown reason";
            GD.PushWarning(reason);
            this._frontierManager.Call("remove_edge_from_frontier", frontier, nextEdge);
            this.update_portals();
            return;
        }

        Godot.Collections.Array validPieces = this._safe_array(validation["valid_pieces"]);
        Variant edgeToConnect = validation["edge_to_connect"];
        bool placed = this._try_place_on_edge(frontier, nextEdge, candidateTile, validPieces, edgeToConnect);
        if (!placed)
        {
            GD.PushWarning($"frontier={frontier} edge={nextEdge} tile={candidateTile} no fitting piece -> removing edge");
            this._frontierManager.Call("remove_edge_from_frontier", frontier, nextEdge);
            this.update_portals();
            return;
        }

        this._frontierManager.Call("prune_all_frontiers");
        this.update_portals();
    }

    public Godot.Collections.Array<Vector2> get_waypoints_for_spawn(Godot.Collections.Dictionary spawn_entry)
    {
        if (this._routeBuilder == null)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        return this._routeBuilder.Call("get_waypoints_for_spawn", spawn_entry).AsGodotArray<Vector2>();
    }

    public void update_portals()
    {
        this.portal_entries.Clear();

        for (int index = 0; index < this.finalized_portal_entries.Count; index++)
        {
            this.portal_entries.Add(this.finalized_portal_entries[index]);
        }

        Godot.Collections.Array frontiers = this._safe_array(this._frontierManager.Call("get_all_frontiers"));
        for (int index = 0; index < frontiers.Count; index++)
        {
            GodotObject frontier = frontiers[index].AsGodotObject();
            if (frontier == null)
            {
                continue;
            }

            Godot.Collections.Array edges = this._safe_array(frontier.Get("edges"));
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                Variant edge = edges[edgeIndex];
                int dir = edge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
                int pos = edge.AsGodotObject()?.Get("pos").AsInt32() ?? 0;

                Vector2I logicalPos = frontier.Get("logical_pos").AsVector2I();
                Vector2I tile = this._gridManager.Call("get_neighbor_tile", logicalPos, dir).AsVector2I();
                Vector2 worldPos = frontier.Get("global_position").AsVector2()
                    + frontier.Call("get_edge_tile_pos", dir, pos).AsVector2()
                    + PortalOffset;
                string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

                var entry = new Godot.Collections.Dictionary
                {
                    { "key", key },
                    { "tile", tile },
                    { "pos", worldPos },
                    { "edge", edge },
                    { "piece", frontier },
                };
                this.portal_entries.Add(entry);
            }
        }

        this.portal_spawn_positions.Clear();
        for (int index = 0; index < this.portal_entries.Count; index++)
        {
            this.portal_spawn_positions.Add(this.portal_entries[index]["pos"].AsVector2());
        }

        if (this._spawnHandler != null)
        {
            this._spawnHandler.Call("update", this.portal_entries);
            this.portal_spawn_positions = this._spawnHandler.Call("get_positions").AsGodotArray<Vector2>();
        }
    }

    private void _on_edge_finalized(Variant piece, Variant edge)
    {
        this._finalize_spawn_pos(piece.AsGodotObject(), edge);
    }

    private bool _try_place_on_edge(GodotObject frontier, Variant nextEdge, Vector2I candidateTile, Godot.Collections.Array validPieces, Variant edgeToConnect)
    {
        Godot.Collections.Array candidatePieces = this._build_candidate_pieces(validPieces);

        for (int index = 0; index < candidatePieces.Count; index++)
        {
            GodotObject pieceData = candidatePieces[index].AsGodotObject();
            GodotObject newPiece = pieceData?.Call("get_instance").AsGodotObject();
            if (newPiece == null)
            {
                continue;
            }

            AddChild(newPiece as Node);

            Godot.Collections.Dictionary occSim = this._gridManager.Call("create_simulated_occupation", candidateTile).AsGodotDictionary();
            Godot.Collections.Array remainingEdges = this._safe_array(newPiece.Get("edges")).Duplicate();

            for (int i = remainingEdges.Count - 1; i >= 0; i--)
            {
                GodotObject remainingEdge = remainingEdges[i].AsGodotObject();
                if (remainingEdge != null && remainingEdge.Call("matches", edgeToConnect).AsBool())
                {
                    remainingEdges.RemoveAt(i);
                    break;
                }
            }

            bool hasOpenPath = false;
            for (int i = 0; i < remainingEdges.Count; i++)
            {
                GodotObject edgeObj = remainingEdges[i].AsGodotObject();
                if (edgeObj == null)
                {
                    continue;
                }

                int dir = edgeObj.Get("dir").AsInt32();
                Vector2I neigh = candidateTile + this._gridManager.Call("get_offset", dir).AsVector2I();
                Variant neighKey = this._gridManager.Call("vec_key", neigh);
                if (occSim.ContainsKey(neighKey))
                {
                    continue;
                }

                if (this._gridManager.Call("reachable_to_boundary", neigh, occSim).AsBool())
                {
                    hasOpenPath = true;
                    break;
                }
            }

            if (!hasOpenPath)
            {
                (newPiece as Node)?.QueueFree();
                continue;
            }

            newPiece.Set("logical_pos", candidateTile);
            this._gridManager.Call("occupy", candidateTile);
            this.composite_tile_map?.register_piece(newPiece);

            frontier.Call("set_edge_has_connected", nextEdge);
            newPiece.Call("set_edge_has_connected", edgeToConnect);

            this._frontierManager.Call("update_after_placement", frontier, newPiece);

            int entryDir = nextEdge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
            int exitDir = edgeToConnect.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
            this._attach_piece(frontier, newPiece, entryDir, exitDir);
            this._move_piece_decoration_to_visuals(newPiece);
            this._lastPieceAttached = newPiece;
            if (!this._hasPlacedFirstExpansion)
            {
                this._hasPlacedFirstExpansion = true;
            }

            if (this._pendingForkAfterBoss && pieceData.Get("is_fork").AsBool())
            {
                this._pendingForkAfterBoss = false;
            }

            return true;
        }

        return false;
    }

    private void _attach_piece(GodotObject pieceA, GodotObject pieceB, int entryDir, int exitDir)
    {
        Vector2 aWorld = pieceA.Call("get_edge_tile_pos", entryDir).AsVector2();
        Vector2 bWorld = pieceB.Call("get_edge_tile_pos", exitDir).AsVector2();
        Vector2I delta = pieceA.Call("get_edge_tile_delta", entryDir).AsVector2I();
        Vector2 shift = pieceA.Call("get_tile_local_offset", delta).AsVector2();
        Vector2 pieceAPosition = pieceA.Get("global_position").AsVector2();
        pieceB.Set("global_position", pieceAPosition + aWorld - bWorld + shift);

        this._connectionGraph.Call("connect_pieces", pieceA, pieceB, entryDir, exitDir);
    }

    private void _move_piece_decoration_to_visuals(GodotObject piece)
    {
        if (piece == null)
        {
            return;
        }

        if (this.visual == null)
        {
            GD.PushWarning($"[WorldMap] visual container is null; cannot move decoration for piece {piece.Get("name")}");
            return;
        }

        Node2D decoration = piece.Call("get_decoration").AsGodotObject() as Node2D;
        if (decoration == null)
        {
            return;
        }

        List<Node2D> leafNodes = new();
        this._collect_leaf_node2d(decoration, leafNodes);

        for (int index = 0; index < leafNodes.Count; index++)
        {
            Node2D leaf = leafNodes[index];
            if (leaf == decoration || leaf.GetParent() == this.visual)
            {
                continue;
            }

            leaf.Reparent(this.visual, true);
        }
    }

    private void _collect_leaf_node2d(Node node, List<Node2D> output)
    {
        if (node == null)
        {
            return;
        }

        Godot.Collections.Array<Node> children = node.GetChildren();
        if (children.Count == 0)
        {
            if (node is Node2D node2D)
            {
                output.Add(node2D);
            }

            return;
        }

        for (int index = 0; index < children.Count; index++)
        {
            this._collect_leaf_node2d(children[index], output);
        }
    }

    private void _finalize_spawn_pos(GodotObject piece, Variant edge)
    {
        if (piece == null)
        {
            return;
        }

        int dir = edge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
        int pos = edge.AsGodotObject()?.Get("pos").AsInt32() ?? 0;
        Vector2I logicalPos = piece.Get("logical_pos").AsVector2I();

        Vector2I tile = this._gridManager.Call("get_neighbor_tile", logicalPos, dir).AsVector2I();
        Vector2 position = piece.Get("global_position").AsVector2()
            + piece.Call("get_edge_tile_pos", dir, pos).AsVector2()
            + PortalOffset;
        string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

        for (int index = 0; index < this.finalized_portal_entries.Count; index++)
        {
            Godot.Collections.Dictionary existing = this.finalized_portal_entries[index];
            if (existing.ContainsKey("key") && existing["key"].AsString() == key)
            {
                return;
            }
        }

        var entry = new Godot.Collections.Dictionary
        {
            { "key", key },
            { "tile", tile },
            { "pos", position },
            { "edge", edge },
            { "piece", piece },
        };

        this.finalized_portal_entries.Add(entry);
    }

    private void _on_wave_finished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int currentWave = runContext.progress.current_wave;

        if (!this.enable_fork && currentWave % WavesPerBoss == 0)
        {
            this._pendingForkAfterBoss = true;
        }

        if (currentWave % 3 == 0)
        {
            this.attach_next_piece();
        }
    }

    private Godot.Collections.Array _build_candidate_pieces(Godot.Collections.Array validPieces)
    {
        Godot.Collections.Array candidatePieces = new();

        if (!this._hasPlacedFirstExpansion)
        {
            this._append_filtered_by_fork(validPieces, candidatePieces, false);
            candidatePieces.Shuffle();
            return candidatePieces;
        }

        if (this.enable_fork)
        {
            candidatePieces = validPieces.Duplicate();
            candidatePieces.Shuffle();
            return candidatePieces;
        }

        if (this._pendingForkAfterBoss)
        {
            Godot.Collections.Array forkPieces = new();
            Godot.Collections.Array otherPieces = new();
            this._append_filtered_by_fork(validPieces, forkPieces, true);
            this._append_filtered_by_fork(validPieces, otherPieces, false);
            forkPieces.Shuffle();
            otherPieces.Shuffle();
            for (int index = 0; index < forkPieces.Count; index++)
            {
                candidatePieces.Add(forkPieces[index]);
            }

            for (int index = 0; index < otherPieces.Count; index++)
            {
                candidatePieces.Add(otherPieces[index]);
            }

            return candidatePieces;
        }

        this._append_filtered_by_fork(validPieces, candidatePieces, false);
        candidatePieces.Shuffle();
        return candidatePieces;
    }

    private void _append_filtered_by_fork(Godot.Collections.Array source, Godot.Collections.Array target, bool isFork)
    {
        for (int index = 0; index < source.Count; index++)
        {
            GodotObject pieceData = source[index].AsGodotObject();
            if (pieceData != null && pieceData.Get("is_fork").AsBool() == isFork)
            {
                target.Add(source[index]);
            }
        }
    }

    private Variant _pick_random(Godot.Collections.Array array)
    {
        if (array.Count == 0)
        {
            return default;
        }

        int index = (int)(GD.Randi() % (uint)array.Count);
        return array[index];
    }

    private Godot.Collections.Array _safe_array(Variant value)
    {
        if (value.VariantType == Variant.Type.Array)
        {
            return value.AsGodotArray();
        }

        return new Godot.Collections.Array();
    }

    private Variant _frontier_manager_pick_random_edge(GodotObject frontier)
    {
        Script frontierManagerScript = GD.Load<Script>("res://levels/word_builder/frontier_manager.gd");
        return frontierManagerScript.Call("pick_random_edge", frontier);
    }
}
