using Godot;
using System.Collections.Generic;
using System.Linq;

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

    private GridManager _gridManager;
    private IWordBuilderAdapter _wordBuilderAdapter;
    private PieceConnectionGraph _connectionGraph;
    private FrontierManager _frontierManager;
    private RouteBuilder _routeBuilder;
    private SpawnPositionsHandler _spawnHandler;

    private RunProgress _progress;

    private sealed class GodotWordBuilderAdapter : IWordBuilderAdapter
    {
        private static GodotObject ToGodotObject(object value)
        {
            if (value is GodotObject godotObject)
            {
                return godotObject;
            }

            if (value is Variant variant && variant.VariantType == Variant.Type.Object)
            {
                return variant.AsGodotObject();
            }

            return null;
        }

        public long GetObjectKey(object value)
        {
            GodotObject godotObject = ToGodotObject(value);
            return godotObject != null ? unchecked((long)godotObject.GetInstanceId()) : 0;
        }

        public bool IsPieceValid(object piece)
        {
            GodotObject godotObject = ToGodotObject(piece);
            return godotObject != null && GodotObject.IsInstanceValid(godotObject);
        }

        public IList<object> GetPieceEdges(object piece)
        {
            var result = new List<object>();
            GodotObject godotObject = ToGodotObject(piece);
            if (godotObject == null)
            {
                return result;
            }

            Godot.Collections.Array edges = godotObject.Get("edges").AsGodotArray();
            for (int i = 0; i < edges.Count; i++)
            {
                result.Add(edges[i].AsGodotObject());
            }

            return result;
        }

        public bool RemoveEdgeFromPiece(object piece, object edge)
        {
            GodotObject pieceObject = ToGodotObject(piece);
            GodotObject edgeObject = ToGodotObject(edge);
            if (pieceObject == null || edgeObject == null)
            {
                return false;
            }

            Godot.Collections.Array edges = pieceObject.Get("edges").AsGodotArray();
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                GodotObject currentEdge = edges[i].AsGodotObject();
                if (currentEdge != null && currentEdge.Call("matches", Variant.From(edgeObject)).AsBool())
                {
                    edges.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public Vector2I GetPieceLogicalPos(object piece)
        {
            GodotObject godotObject = ToGodotObject(piece);
            return godotObject != null ? godotObject.Get("logical_pos").AsVector2I() : Vector2I.Zero;
        }

        public int GetEdgeDir(object edge)
        {
            GodotObject edgeObject = ToGodotObject(edge);
            return edgeObject != null ? edgeObject.Get("dir").AsInt32() : 0;
        }

        public int GetEdgePos(object edge)
        {
            GodotObject edgeObject = ToGodotObject(edge);
            return edgeObject != null ? edgeObject.Get("pos").AsInt32() : 0;
        }

        public object GetOppositeEdge(object edge)
        {
            GodotObject edgeObject = ToGodotObject(edge);
            return edgeObject != null ? edgeObject.Call("get_opposite").AsGodotObject() : null;
        }

        public bool EdgesMatch(object leftEdge, object rightEdge)
        {
            GodotObject left = ToGodotObject(leftEdge);
            GodotObject right = ToGodotObject(rightEdge);
            return left != null && right != null && left.Call("matches", Variant.From(right)).AsBool();
        }

        public bool PieceDataHasConnectingEdge(object pieceData, object edge)
        {
            GodotObject data = ToGodotObject(pieceData);
            GodotObject edgeObject = ToGodotObject(edge);
            return data != null && edgeObject != null && data.Call("has_connecting_edge", Variant.From(edgeObject)).AsBool();
        }

        public bool PieceDataHasEdgeDir(object pieceData, int dir)
        {
            GodotObject data = ToGodotObject(pieceData);
            return data != null && data.Call("has_edge_dir", dir).AsBool();
        }

        public Vector2 GetPieceGlobalPosition(object piece)
        {
            GodotObject godotObject = ToGodotObject(piece);
            return godotObject != null ? godotObject.Get("global_position").AsVector2() : Vector2.Zero;
        }

        public IList<Vector2> GetRouteWaypoints(object piece, int entryDir, int exitDir)
        {
            if (piece is MapPiece mapPiece)
            {
                return mapPiece.get_route_waypoints(entryDir, exitDir);
            }

            GodotObject godotObject = ToGodotObject(piece);
            if (godotObject != null)
            {
                if (godotObject.HasMethod("get_route_waypoints"))
                {
                    return godotObject.Call("get_route_waypoints", entryDir, exitDir).AsGodotArray<Vector2>();
                }

                if (godotObject.HasMethod("GetRouteWaypoints"))
                {
                    return godotObject.Call("GetRouteWaypoints", entryDir, exitDir).AsGodotArray<Vector2>();
                }
            }

            return new List<Vector2>();
        }

        public IList<Vector2> GetFinalRouteWaypoints(object piece, int entryDir)
        {
            if (piece is MapPiece mapPiece)
            {
                return mapPiece.get_final_route_waypoints(entryDir);
            }

            GodotObject godotObject = ToGodotObject(piece);
            if (godotObject != null)
            {
                if (godotObject.HasMethod("get_final_route_waypoints"))
                {
                    return godotObject.Call("get_final_route_waypoints", entryDir).AsGodotArray<Vector2>();
                }

                if (godotObject.HasMethod("GetFinalRouteWaypoints"))
                {
                    return godotObject.Call("GetFinalRouteWaypoints", entryDir).AsGodotArray<Vector2>();
                }
            }

            return new List<Vector2>();
        }

        public int GetOppositeDir(int dir)
        {
            return (int)Edge.get_opposite_dir((Edge.Dir)dir);
        }

        public bool PieceDataIsFork(object pieceData)
        {
            GodotObject data = ToGodotObject(pieceData);
            return data != null && data.Get("is_fork").AsBool();
        }
    }
    public override void _Ready()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        this._mapPieces = dataLoader.get_all_map_pieces();

        this._gridManager = new GridManager();
        this._wordBuilderAdapter = new GodotWordBuilderAdapter();
        this._connectionGraph = new PieceConnectionGraph(this._wordBuilderAdapter);
        this._frontierManager = new FrontierManager(this._wordBuilderAdapter);
        this._frontierManager.setup(this._gridManager, this._to_object_list(this._mapPieces));
        this._spawnHandler = new SpawnPositionsHandler();
        this._spawnHandler.setup(this.visual);

        if (this._gridManager == null || this._connectionGraph == null || this._spawnHandler == null)
        {
            GD.PushError("[WorldMap] Failed to initialize world builder managers.");
            return;
        }

        this._frontierManager.edge_finalized += this._on_edge_finalized;

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

        this._gridManager.occupy(Vector2I.Zero);
        this._connectionGraph.register_piece(initPiece);
        this.composite_tile_map?.register_piece(initPiece);
        this._frontierManager.add_frontier(initPiece);

        this._routeBuilder = new RouteBuilder(this._wordBuilderAdapter);
        this._routeBuilder.setup(this._connectionGraph, initPiece);

        this._lastPieceAttached = initPiece;
        this.update_portals();
        this.attach_next_piece();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext.progress;
        this._progress.current_wave_finished += this._on_wave_finished;
    }

    public override void _ExitTree()
    {
        if (this._frontierManager != null)
        {
            this._frontierManager.edge_finalized -= this._on_edge_finalized;
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
        if (!this._frontierManager.has_frontiers())
        {
            GD.PushError("No frontiers available for placement");
            return;
        }

        GodotObject frontier = this._frontierManager.select_random_frontier() as GodotObject;
        if (frontier == null)
        {
            GD.PushError("Failed to select frontier");
            return;
        }

        Godot.Collections.Array frontierEdges = this._safe_array(frontier.Get("edges"));
        if (frontierEdges.Count == 0)
        {
            this._frontierManager.remove_frontier(frontier);
            this.update_portals();
            return;
        }

        object nextEdge = this._frontier_manager_pick_random_edge(frontier);
        Vector2I frontierLogicalPos = frontier.Get("logical_pos").AsVector2I();
        int nextEdgeDir = (nextEdge as GodotObject)?.Get("dir").AsInt32() ?? 0;
        Vector2I candidateTile = this._gridManager.get_neighbor_tile(frontierLogicalPos, nextEdgeDir);

        FrontierManager.EdgeValidationResult validation = this._frontierManager.validate_edge(frontier, nextEdge, candidateTile);
        if (!validation.Valid)
        {
            string reason = string.IsNullOrEmpty(validation.Reason) ? "unknown reason" : validation.Reason;
            GD.PushWarning(reason);
            this._frontierManager.remove_edge_from_frontier(frontier, nextEdge);
            this.update_portals();
            return;
        }

        List<object> validPieces = validation.ValidPieces;
        object edgeToConnect = validation.EdgeToConnect;
        bool placed = this._try_place_on_edge(frontier, nextEdge, candidateTile, validPieces, edgeToConnect);
        if (!placed)
        {
            GD.PushWarning($"frontier={frontier} edge={nextEdge} tile={candidateTile} no fitting piece -> removing edge");
            this._frontierManager.remove_edge_from_frontier(frontier, nextEdge);
            this.update_portals();
            return;
        }

        this._frontierManager.prune_all_frontiers();
        this.update_portals();
    }

    public Godot.Collections.Array<Vector2> get_waypoints_for_spawn(Godot.Collections.Dictionary spawn_entry)
    {
        if (this._routeBuilder == null)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Dictionary<string, object> spawnEntry = this._to_cs_object_dict(spawn_entry);
        List<Vector2> waypoints = this._routeBuilder.get_waypoints_for_spawn(spawnEntry);
        return new Godot.Collections.Array<Vector2>(waypoints.ToArray());
    }

    public void update_portals()
    {
        this.portal_entries.Clear();

        for (int index = 0; index < this.finalized_portal_entries.Count; index++)
        {
            this.portal_entries.Add(this.finalized_portal_entries[index]);
        }

        List<object> frontiers = this._frontierManager.get_all_frontiers();
        for (int index = 0; index < frontiers.Count; index++)
        {
            GodotObject frontier = frontiers[index] as GodotObject;
            if (frontier == null)
            {
                continue;
            }

            Godot.Collections.Array edges = this._safe_array(frontier.Get("edges"));
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                object edge = edges[edgeIndex].AsGodotObject();
                int dir = (edge as GodotObject)?.Get("dir").AsInt32() ?? 0;
                int pos = (edge as GodotObject)?.Get("pos").AsInt32() ?? 0;

                Vector2I logicalPos = frontier.Get("logical_pos").AsVector2I();
                Vector2I tile = this._gridManager.get_neighbor_tile(logicalPos, dir);
                Vector2 worldPos = frontier.Get("global_position").AsVector2()
                    + this._piece_get_edge_tile_pos(frontier, dir, pos)
                    + PortalOffset;
                string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

                var entry = new Godot.Collections.Dictionary
                {
                    { "key", key },
                    { "tile", tile },
                    { "pos", worldPos },
                    { "edge", Variant.From(edge as GodotObject) },
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
            this._spawnHandler.update(this.portal_entries);
            this.portal_spawn_positions = this._spawnHandler.get_positions();
        }
    }

    private void _on_edge_finalized(object piece, object edge)
    {
        this._finalize_spawn_pos(piece as GodotObject, edge);
    }

    private bool _try_place_on_edge(GodotObject frontier, object nextEdge, Vector2I candidateTile, List<object> validPieces, object edgeToConnect)
    {
        List<object> candidatePieces = this._build_candidate_pieces(validPieces);

        for (int index = 0; index < candidatePieces.Count; index++)
        {
            GodotObject pieceData = candidatePieces[index] as GodotObject;
            GodotObject newPiece = pieceData?.Call("get_instance").AsGodotObject();
            if (newPiece == null)
            {
                continue;
            }

            AddChild(newPiece as Node);

            HashSet<string> occSim = this._gridManager.create_simulated_occupation(candidateTile);
            Godot.Collections.Array remainingEdges = this._safe_array(newPiece.Get("edges")).Duplicate();

            for (int i = remainingEdges.Count - 1; i >= 0; i--)
            {
                GodotObject remainingEdge = remainingEdges[i].AsGodotObject();
                if (remainingEdge != null && remainingEdge.Call("matches", edgeToConnect as GodotObject).AsBool())
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
                Vector2I neigh = candidateTile + this._gridManager.get_offset(dir);
                string neighKey = GridManager.vec_key(neigh);
                if (occSim.Contains(neighKey))
                {
                    continue;
                }

                if (this._gridManager.reachable_to_boundary(neigh, occSim))
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
            this._gridManager.occupy(candidateTile);
            this.composite_tile_map?.register_piece(newPiece);

            this._piece_set_edge_has_connected(frontier, nextEdge);
            this._piece_set_edge_has_connected(newPiece, edgeToConnect);

            this._frontierManager.update_after_placement(frontier, newPiece);

            int entryDir = (nextEdge as GodotObject)?.Get("dir").AsInt32() ?? 0;
            int exitDir = (edgeToConnect as GodotObject)?.Get("dir").AsInt32() ?? 0;
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
        Vector2 aWorld = this._piece_get_edge_tile_pos(pieceA, entryDir);
        Vector2 bWorld = this._piece_get_edge_tile_pos(pieceB, exitDir);
        Vector2I delta = this._piece_get_edge_tile_delta(pieceA, entryDir);
        Vector2 shift = this._piece_get_tile_local_offset(pieceA, delta);
        Vector2 pieceAPosition = pieceA.Get("global_position").AsVector2();
        pieceB.Set("global_position", pieceAPosition + aWorld - bWorld + shift);

        this._connectionGraph.connect_pieces(pieceA, pieceB, entryDir, exitDir);
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

        Node2D decoration = this._piece_get_decoration(piece);
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

    private void _finalize_spawn_pos(GodotObject piece, object edge)
    {
        if (piece == null)
        {
            return;
        }

        int dir = (edge as GodotObject)?.Get("dir").AsInt32() ?? 0;
        int pos = (edge as GodotObject)?.Get("pos").AsInt32() ?? 0;
        Vector2I logicalPos = piece.Get("logical_pos").AsVector2I();

        Vector2I tile = this._gridManager.get_neighbor_tile(logicalPos, dir);
        Vector2 position = piece.Get("global_position").AsVector2()
            + this._piece_get_edge_tile_pos(piece, dir, pos)
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
            { "edge", Variant.From(edge as GodotObject) },
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

    private List<object> _build_candidate_pieces(List<object> validPieces)
    {
        var candidatePieces = new List<object>();

        if (!this._hasPlacedFirstExpansion)
        {
            this._append_filtered_by_fork(validPieces, candidatePieces, false);
            this._shuffle(candidatePieces);
            return candidatePieces;
        }

        if (this.enable_fork)
        {
            candidatePieces = new List<object>(validPieces);
            this._shuffle(candidatePieces);
            return candidatePieces;
        }

        if (this._pendingForkAfterBoss)
        {
            var forkPieces = new List<object>();
            var otherPieces = new List<object>();
            this._append_filtered_by_fork(validPieces, forkPieces, true);
            this._append_filtered_by_fork(validPieces, otherPieces, false);
            this._shuffle(forkPieces);
            this._shuffle(otherPieces);
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
        this._shuffle(candidatePieces);
        return candidatePieces;
    }

    private static bool _has_method(GodotObject target, string methodName)
    {
        return target != null && GodotObject.IsInstanceValid(target) && target.HasMethod(methodName);
    }

    private Vector2 _piece_get_edge_tile_pos(GodotObject piece, int dir, int pos = 1)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_edge_tile_pos(dir, pos);
        }

        if (_has_method(piece, "get_edge_tile_pos"))
        {
            return piece.Call("get_edge_tile_pos", dir, pos).AsVector2();
        }

        if (_has_method(piece, "GetEdgeTilePos"))
        {
            return piece.Call("GetEdgeTilePos", dir, pos).AsVector2();
        }

        GD.PushError("[WorldMap] Piece has no edge position method.");
        return Vector2.Zero;
    }

    private Vector2I _piece_get_edge_tile_delta(GodotObject piece, int dir)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_edge_tile_delta(dir);
        }

        if (_has_method(piece, "get_edge_tile_delta"))
        {
            return piece.Call("get_edge_tile_delta", dir).AsVector2I();
        }

        if (_has_method(piece, "GetEdgeTileDelta"))
        {
            return piece.Call("GetEdgeTileDelta", dir).AsVector2I();
        }

        GD.PushError("[WorldMap] Piece has no edge tile delta method.");
        return Vector2I.Zero;
    }

    private Vector2 _piece_get_tile_local_offset(GodotObject piece, Vector2I delta)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_tile_local_offset(delta);
        }

        if (_has_method(piece, "get_tile_local_offset"))
        {
            return piece.Call("get_tile_local_offset", delta).AsVector2();
        }

        if (_has_method(piece, "GetTileLocalOffset"))
        {
            return piece.Call("GetTileLocalOffset", delta).AsVector2();
        }

        GD.PushError("[WorldMap] Piece has no local offset method.");
        return Vector2.Zero;
    }

    private Node2D _piece_get_decoration(GodotObject piece)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_decoration();
        }

        if (_has_method(piece, "get_decoration"))
        {
            return piece.Call("get_decoration").AsGodotObject() as Node2D;
        }

        if (_has_method(piece, "GetDecoration"))
        {
            return piece.Call("GetDecoration").AsGodotObject() as Node2D;
        }

        return null;
    }

    private void _piece_set_edge_has_connected(GodotObject piece, object edge)
    {
        if (piece is MapPiece mapPiece)
        {
            mapPiece.set_edge_has_connected(edge as Edge);
            return;
        }

        if (_has_method(piece, "set_edge_has_connected"))
        {
            piece.Call("set_edge_has_connected", Variant.From(edge as GodotObject));
            return;
        }

        if (_has_method(piece, "SetEdgeHasConnected"))
        {
            piece.Call("SetEdgeHasConnected", Variant.From(edge as GodotObject));
            return;
        }

        GD.PushError("[WorldMap] Piece has no set_edge_has_connected method.");
    }

    private void _append_filtered_by_fork(List<object> source, List<object> target, bool isFork)
    {
        for (int index = 0; index < source.Count; index++)
        {
            GodotObject pieceData = source[index] as GodotObject;
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

    private object _frontier_manager_pick_random_edge(GodotObject frontier)
    {
        return FrontierManager.pick_random_edge(frontier, this._wordBuilderAdapter);
    }

    private void _shuffle(List<object> items)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    private Dictionary<string, Variant> _to_cs_dict(Godot.Collections.Dictionary source)
    {
        var result = new Dictionary<string, Variant>();
        foreach (Variant key in source.Keys)
        {
            result[key.AsString()] = source[key];
        }

        return result;
    }

    private List<Dictionary<string, Variant>> _to_cs_entries(Godot.Collections.Array<Godot.Collections.Dictionary> entries)
    {
        var result = new List<Dictionary<string, Variant>>(entries.Count);
        for (int i = 0; i < entries.Count; i++)
        {
            result.Add(this._to_cs_dict(entries[i]));
        }

        return result;
    }

    private Dictionary<string, object> _to_cs_object_dict(Godot.Collections.Dictionary source)
    {
        var result = new Dictionary<string, object>();
        foreach (Variant key in source.Keys)
        {
            string stringKey = key.AsString();
            Variant value = source[key];

            object rawValue = value.VariantType switch
            {
                Variant.Type.String => value.AsString(),
                Variant.Type.Int => value.AsInt32(),
                Variant.Type.Float => value.AsSingle(),
                Variant.Type.Vector2 => value.AsVector2(),
                Variant.Type.Vector2I => value.AsVector2I(),
                Variant.Type.Object => value.AsGodotObject(),
                _ => value,
            };

            result[stringKey] = rawValue;
        }

        return result;
    }

    private List<Dictionary<string, object>> _to_cs_entries_object(Godot.Collections.Array<Godot.Collections.Dictionary> entries)
    {
        var result = new List<Dictionary<string, object>>(entries.Count);
        for (int i = 0; i < entries.Count; i++)
        {
            result.Add(this._to_cs_object_dict(entries[i]));
        }

        return result;
    }

    private List<object> _to_object_list(Godot.Collections.Array<Variant> source)
    {
        var result = new List<object>(source.Count);
        for (int i = 0; i < source.Count; i++)
        {
            result.Add(source[i].AsGodotObject());
        }

        return result;
    }
}
