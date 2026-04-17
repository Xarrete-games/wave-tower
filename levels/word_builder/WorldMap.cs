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
    private MapPiece _lastPieceAttached;
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
        private static MapPiece AsMapPiece(object value)
        {
            if (value is MapPiece piece)
            {
                return piece;
            }

            if (value is Variant variant && variant.VariantType == Variant.Type.Object)
            {
                return variant.AsGodotObject() as MapPiece;
            }

            return null;
        }

        private static Edge AsEdge(object value)
        {
            if (value is Edge edge)
            {
                return edge;
            }

            if (value is Variant variant && variant.VariantType == Variant.Type.Object)
            {
                return variant.AsGodotObject() as Edge;
            }

            return null;
        }

        private static MapPieceData AsPieceData(object value)
        {
            if (value is MapPieceData pieceData)
            {
                return pieceData;
            }

            if (value is Variant variant && variant.VariantType == Variant.Type.Object)
            {
                return variant.AsGodotObject() as MapPieceData;
            }

            return null;
        }

        public long GetObjectKey(object value)
        {
            MapPiece piece = AsMapPiece(value);
            return piece != null ? unchecked((long)piece.GetInstanceId()) : 0;
        }

        public bool IsPieceValid(object piece)
        {
            MapPiece mapPiece = AsMapPiece(piece);
            return mapPiece != null && GodotObject.IsInstanceValid(mapPiece);
        }

        public IList<object> GetPieceEdges(object piece)
        {
            var result = new List<object>();
            MapPiece mapPiece = AsMapPiece(piece);
            if (mapPiece == null)
            {
                return result;
            }

            Godot.Collections.Array<Edge> edges = mapPiece.edges;
            for (int i = 0; i < edges.Count; i++)
            {
                result.Add(edges[i]);
            }

            return result;
        }

        public bool RemoveEdgeFromPiece(object piece, object edge)
        {
            MapPiece pieceObject = AsMapPiece(piece);
            Edge edgeObject = AsEdge(edge);
            if (pieceObject == null || edgeObject == null)
            {
                return false;
            }

            Godot.Collections.Array<Edge> edges = pieceObject.edges;
            for (int i = edges.Count - 1; i >= 0; i--)
            {
                Edge currentEdge = edges[i];
                if (currentEdge != null && currentEdge.matches(edgeObject))
                {
                    edges.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public Vector2I GetPieceLogicalPos(object piece)
        {
            MapPiece mapPiece = AsMapPiece(piece);
            return mapPiece != null ? mapPiece.logical_pos : Vector2I.Zero;
        }

        public int GetEdgeDir(object edge)
        {
            Edge edgeObject = AsEdge(edge);
            return edgeObject != null ? (int)edgeObject.dir : 0;
        }

        public int GetEdgePos(object edge)
        {
            Edge edgeObject = AsEdge(edge);
            return edgeObject != null ? (int)edgeObject.pos : 0;
        }

        public object GetOppositeEdge(object edge)
        {
            Edge edgeObject = AsEdge(edge);
            return edgeObject?.get_opposite();
        }

        public bool EdgesMatch(object leftEdge, object rightEdge)
        {
            Edge left = AsEdge(leftEdge);
            Edge right = AsEdge(rightEdge);
            return left != null && right != null && left.matches(right);
        }

        public bool PieceDataHasConnectingEdge(object pieceData, object edge)
        {
            MapPieceData data = AsPieceData(pieceData);
            Edge edgeObject = AsEdge(edge);
            return data != null && edgeObject != null && data.has_connecting_edge(edgeObject);
        }

        public bool PieceDataHasEdgeDir(object pieceData, int dir)
        {
            MapPieceData data = AsPieceData(pieceData);
            return data != null && data.has_edge_dir(dir);
        }

        public Vector2 GetPieceGlobalPosition(object piece)
        {
            MapPiece mapPiece = AsMapPiece(piece);
            return mapPiece != null ? mapPiece.GlobalPosition : Vector2.Zero;
        }

        public IList<Vector2> GetRouteWaypoints(object piece, int entryDir, int exitDir)
        {
            MapPiece mapPiece = AsMapPiece(piece);
            return mapPiece != null ? mapPiece.get_route_waypoints(entryDir, exitDir) : new List<Vector2>();
        }

        public IList<Vector2> GetFinalRouteWaypoints(object piece, int entryDir)
        {
            MapPiece mapPiece = AsMapPiece(piece);
            return mapPiece != null ? mapPiece.get_final_route_waypoints(entryDir) : new List<Vector2>();
        }

        public int GetOppositeDir(int dir)
        {
            return (int)Edge.get_opposite_dir((Edge.Dir)dir);
        }

        public bool PieceDataIsFork(object pieceData)
        {
            MapPieceData data = AsPieceData(pieceData);
            return data != null && data.is_fork;
        }
    }
    public override void _Ready()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        _mapPieces = dataLoader.get_all_map_pieces();

        _gridManager = new GridManager();
        _wordBuilderAdapter = new GodotWordBuilderAdapter();
        _connectionGraph = new PieceConnectionGraph(_wordBuilderAdapter);
        _frontierManager = new FrontierManager(_wordBuilderAdapter);
        _frontierManager.setup(_gridManager, ToObjectList(_mapPieces));
        _spawnHandler = new SpawnPositionsHandler();
        _spawnHandler.setup(visual);

        if (_gridManager == null || _connectionGraph == null || _spawnHandler == null)
        {
            GD.PushError("[WorldMap] Failed to initialize world builder managers.");
            return;
        }

        _frontierManager.edge_finalized += OnEdgeFinalized;

        MapPieceData initPieceData = PickRandom(_safe_array(dataLoader.get_all_initial_map_pieces())).AsGodotObject() as MapPieceData;
        MapPiece initPiece = initPieceData?.get_instance().AsGodotObject() as MapPiece;
        if (initPiece == null)
        {
            GD.PushError("[WorldMap] Could not instantiate initial piece.");
            return;
        }

        AddChild(initPiece);
        initPiece.logical_pos = Vector2I.Zero;
        MovePieceDecorationToVisuals(initPiece);

        _gridManager.occupy(Vector2I.Zero);
        _connectionGraph.register_piece(initPiece);
        composite_tile_map?.register_piece(initPiece);
        _frontierManager.add_frontier(initPiece);

        _routeBuilder = new RouteBuilder(_wordBuilderAdapter);
        _routeBuilder.setup(_connectionGraph, initPiece);

        _lastPieceAttached = initPiece;
        update_portals();
        attach_next_piece();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext.progress;
        _progress.current_wave_finished += OnWaveFinished;
    }

    public override void _ExitTree()
    {
        if (_frontierManager != null)
        {
            _frontierManager.edge_finalized -= OnEdgeFinalized;
        }

        if (_progress != null)
        {
            _progress.current_wave_finished -= OnWaveFinished;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test"))
        {
            attach_next_piece();
        }
    }

    public void attach_next_piece()
    {
        if (!_frontierManager.has_frontiers())
        {
            GD.PushError("No frontiers available for placement");
            return;
        }

        MapPiece frontier = _frontierManager.select_random_frontier() as MapPiece;
        if (frontier == null)
        {
            GD.PushError("Failed to select frontier");
            return;
        }

        Godot.Collections.Array<Edge> frontierEdges = frontier.edges;
        if (frontierEdges.Count == 0)
        {
            _frontierManager.remove_frontier(frontier);
            update_portals();
            return;
        }

        Edge nextEdge = FrontierManagerPickRandomEdge(frontier) as Edge;
        Vector2I frontierLogicalPos = frontier.logical_pos;
        int nextEdgeDir = nextEdge != null ? (int)nextEdge.dir : 0;
        Vector2I candidateTile = _gridManager.get_neighbor_tile(frontierLogicalPos, nextEdgeDir);

        FrontierManager.EdgeValidationResult validation = _frontierManager.validate_edge(frontier, nextEdge, candidateTile);
        if (!validation.Valid)
        {
            string reason = string.IsNullOrEmpty(validation.Reason) ? "unknown reason" : validation.Reason;
            GD.PushWarning(reason);
            _frontierManager.remove_edge_from_frontier(frontier, nextEdge);
            update_portals();
            return;
        }

        List<object> validPieces = validation.ValidPieces;
        object edgeToConnect = validation.EdgeToConnect;
        bool placed = TryPlaceOnEdge(frontier, nextEdge, candidateTile, validPieces, edgeToConnect);
        if (!placed)
        {
            GD.PushWarning($"frontier={frontier} edge={nextEdge} tile={candidateTile} no fitting piece -> removing edge");
            _frontierManager.remove_edge_from_frontier(frontier, nextEdge);
            update_portals();
            return;
        }

        _frontierManager.prune_all_frontiers();
        update_portals();
    }

    public Godot.Collections.Array<Vector2> get_waypoints_for_spawn(Godot.Collections.Dictionary spawn_entry)
    {
        if (_routeBuilder == null)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Dictionary<string, object> spawnEntry = _to_cs_object_dict(spawn_entry);
        List<Vector2> waypoints = _routeBuilder.get_waypoints_for_spawn(spawnEntry);
        return new Godot.Collections.Array<Vector2>(waypoints.ToArray());
    }

    public void update_portals()
    {
        portal_entries.Clear();

        for (int index = 0; index < finalized_portal_entries.Count; index++)
        {
            portal_entries.Add(finalized_portal_entries[index]);
        }

        List<object> frontiers = _frontierManager.get_all_frontiers();
        for (int index = 0; index < frontiers.Count; index++)
        {
            MapPiece frontier = frontiers[index] as MapPiece;
            if (frontier == null)
            {
                continue;
            }

            Godot.Collections.Array<Edge> edges = frontier.edges;
            for (int edgeIndex = 0; edgeIndex < edges.Count; edgeIndex++)
            {
                Edge edge = edges[edgeIndex];
                int dir = edge != null ? (int)edge.dir : 0;
                int pos = edge != null ? (int)edge.pos : 0;

                Vector2I logicalPos = frontier.logical_pos;
                Vector2I tile = _gridManager.get_neighbor_tile(logicalPos, dir);
                Vector2 worldPos = frontier.GlobalPosition
                    + PieceGetEdgeTilePos(frontier, dir, pos)
                    + PortalOffset;
                string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

                var entry = new Godot.Collections.Dictionary
                {
                    { "key", key },
                    { "tile", tile },
                    { "pos", worldPos },
                    { "edge", Variant.From(edge) },
                    { "piece", frontier },
                };
                portal_entries.Add(entry);
            }
        }

        portal_spawn_positions.Clear();
        for (int index = 0; index < portal_entries.Count; index++)
        {
            portal_spawn_positions.Add(portal_entries[index]["pos"].AsVector2());
        }

        if (_spawnHandler != null)
        {
            _spawnHandler.update(portal_entries);
            portal_spawn_positions = _spawnHandler.get_positions();
        }
    }

    private void OnEdgeFinalized(object piece, object edge)
    {
        FinalizeSpawnPos(piece as MapPiece, edge as Edge);
    }

    private bool TryPlaceOnEdge(MapPiece frontier, Edge nextEdge, Vector2I candidateTile, List<object> validPieces, object edgeToConnect)
    {
        List<object> candidatePieces = BuildCandidatePieces(validPieces);
        Edge edgeToConnectTyped = edgeToConnect as Edge;

        for (int index = 0; index < candidatePieces.Count; index++)
        {
            MapPieceData pieceData = candidatePieces[index] as MapPieceData;
            MapPiece newPiece = pieceData?.get_instance().AsGodotObject() as MapPiece;
            if (newPiece == null)
            {
                continue;
            }

            AddChild(newPiece);

            HashSet<string> occSim = _gridManager.create_simulated_occupation(candidateTile);
            var remainingEdges = new List<Edge>();
            for (int i = 0; i < newPiece.edges.Count; i++)
            {
                Edge edge = newPiece.edges[i];
                if (edge != null && (edgeToConnectTyped == null || !edge.matches(edgeToConnectTyped)))
                {
                    remainingEdges.Add(edge);
                }
            }

            bool hasOpenPath = false;
            for (int i = 0; i < remainingEdges.Count; i++)
            {
                Edge edgeObj = remainingEdges[i];
                if (edgeObj == null)
                {
                    continue;
                }

                int dir = (int)edgeObj.dir;
                Vector2I neigh = candidateTile + _gridManager.get_offset(dir);
                string neighKey = GridManager.vec_key(neigh);
                if (occSim.Contains(neighKey))
                {
                    continue;
                }

                if (_gridManager.reachable_to_boundary(neigh, occSim))
                {
                    hasOpenPath = true;
                    break;
                }
            }

            if (!hasOpenPath)
            {
                newPiece.QueueFree();
                continue;
            }

            newPiece.logical_pos = candidateTile;
            _gridManager.occupy(candidateTile);
            composite_tile_map?.register_piece(newPiece);

            PieceSetEdgeHasConnected(frontier, nextEdge);
            PieceSetEdgeHasConnected(newPiece, edgeToConnect);

            _frontierManager.update_after_placement(frontier, newPiece);

            int entryDir = nextEdge != null ? (int)nextEdge.dir : 0;
            int exitDir = edgeToConnectTyped != null ? (int)edgeToConnectTyped.dir : 0;
            AttachPiece(frontier, newPiece, entryDir, exitDir);
            MovePieceDecorationToVisuals(newPiece);
            _lastPieceAttached = newPiece;
            if (!_hasPlacedFirstExpansion)
            {
                _hasPlacedFirstExpansion = true;
            }

            if (_pendingForkAfterBoss && pieceData != null && pieceData.is_fork)
            {
                _pendingForkAfterBoss = false;
            }

            return true;
        }

        return false;
    }

    private void AttachPiece(MapPiece pieceA, MapPiece pieceB, int entryDir, int exitDir)
    {
        Vector2 aWorld = PieceGetEdgeTilePos(pieceA, entryDir);
        Vector2 bWorld = PieceGetEdgeTilePos(pieceB, exitDir);
        Vector2I delta = PieceGetEdgeTileDelta(pieceA, entryDir);
        Vector2 shift = PieceGetTileLocalOffset(pieceA, delta);
        Vector2 pieceAPosition = pieceA.GlobalPosition;
        pieceB.GlobalPosition = pieceAPosition + aWorld - bWorld + shift;

        _connectionGraph.connect_pieces(pieceA, pieceB, entryDir, exitDir);
    }

    private void MovePieceDecorationToVisuals(MapPiece piece)
    {
        if (piece == null)
        {
            return;
        }

        if (visual == null)
        {
            GD.PushWarning($"[WorldMap] visual container is null; cannot move decoration for piece {piece.Name}");
            return;
        }

        Node2D decoration = PieceGetDecoration(piece);
        if (decoration == null)
        {
            return;
        }

        List<Node2D> leafNodes = new();
        CollectLeafNode2d(decoration, leafNodes);

        for (int index = 0; index < leafNodes.Count; index++)
        {
            Node2D leaf = leafNodes[index];
            if (leaf == decoration || leaf.GetParent() == visual)
            {
                continue;
            }

            leaf.Reparent(visual, true);
        }
    }

    private void CollectLeafNode2d(Node node, List<Node2D> output)
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
            CollectLeafNode2d(children[index], output);
        }
    }

    private void FinalizeSpawnPos(MapPiece piece, Edge edge)
    {
        if (piece == null)
        {
            return;
        }

        int dir = edge != null ? (int)edge.dir : 0;
        int pos = edge != null ? (int)edge.pos : 0;
        Vector2I logicalPos = piece.logical_pos;

        Vector2I tile = _gridManager.get_neighbor_tile(logicalPos, dir);
        Vector2 position = piece.GlobalPosition
            + PieceGetEdgeTilePos(piece, dir, pos)
            + PortalOffset;
        string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

        for (int index = 0; index < finalized_portal_entries.Count; index++)
        {
            Godot.Collections.Dictionary existing = finalized_portal_entries[index];
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
            { "edge", Variant.From(edge) },
            { "piece", piece },
        };

        finalized_portal_entries.Add(entry);
    }

    private void OnWaveFinished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int currentWave = runContext.progress.current_wave;

        if (!enable_fork && currentWave % WavesPerBoss == 0)
        {
            _pendingForkAfterBoss = true;
        }

        if (currentWave % 3 == 0)
        {
            attach_next_piece();
        }
    }

    private List<object> BuildCandidatePieces(List<object> validPieces)
    {
        var candidatePieces = new List<object>();

        if (!_hasPlacedFirstExpansion)
        {
            AppendFilteredByFork(validPieces, candidatePieces, false);
            Shuffle(candidatePieces);
            return candidatePieces;
        }

        if (enable_fork)
        {
            candidatePieces = new List<object>(validPieces);
            Shuffle(candidatePieces);
            return candidatePieces;
        }

        if (_pendingForkAfterBoss)
        {
            var forkPieces = new List<object>();
            var otherPieces = new List<object>();
            AppendFilteredByFork(validPieces, forkPieces, true);
            AppendFilteredByFork(validPieces, otherPieces, false);
            Shuffle(forkPieces);
            Shuffle(otherPieces);
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

        AppendFilteredByFork(validPieces, candidatePieces, false);
        Shuffle(candidatePieces);
        return candidatePieces;
    }

    private Vector2 PieceGetEdgeTilePos(MapPiece piece, int dir, int pos = 1)
    {
        return piece != null ? piece.get_edge_tile_pos(dir, pos) : Vector2.Zero;
    }

    private Vector2I PieceGetEdgeTileDelta(MapPiece piece, int dir)
    {
        return piece != null ? piece.get_edge_tile_delta(dir) : Vector2I.Zero;
    }

    private Vector2 PieceGetTileLocalOffset(MapPiece piece, Vector2I delta)
    {
        return piece != null ? piece.get_tile_local_offset(delta) : Vector2.Zero;
    }

    private Node2D PieceGetDecoration(MapPiece piece)
    {
        return piece?.get_decoration();
    }

    private void PieceSetEdgeHasConnected(MapPiece piece, object edge)
    {
        piece?.set_edge_has_connected(edge as Edge);
    }

    private void AppendFilteredByFork(List<object> source, List<object> target, bool isFork)
    {
        for (int index = 0; index < source.Count; index++)
        {
            MapPieceData pieceData = source[index] as MapPieceData;
            if (pieceData != null && pieceData.is_fork == isFork)
            {
                target.Add(source[index]);
            }
        }
    }

    private Variant PickRandom(Godot.Collections.Array array)
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

    private object FrontierManagerPickRandomEdge(GodotObject frontier)
    {
        return FrontierManager.pick_random_edge(frontier, _wordBuilderAdapter);
    }

    private void Shuffle(List<object> items)
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
            result.Add(_to_cs_dict(entries[i]));
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
            result.Add(_to_cs_object_dict(entries[i]));
        }

        return result;
    }

    private List<object> ToObjectList(Godot.Collections.Array<Variant> source)
    {
        var result = new List<object>(source.Count);
        for (int i = 0; i < source.Count; i++)
        {
            result.Add(source[i].AsGodotObject() as MapPieceData);
        }

        return result;
    }
}
