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
    public CompositeTileMap CompositeTileMap;

    [Export]
    public bool EnableFork = true;

    private readonly List<SpawnEntry> _portalEntries = new();
    private readonly List<SpawnEntry> _finalizedPortalEntries = new();

    public IReadOnlyList<SpawnEntry> GetPortalEntries()
    {
        return _portalEntries;
    }

    private List<MapPieceData> _mapPieces = new();
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
        public long GetObjectKey(MapPiece value)
        {
            return value != null ? unchecked((long)value.GetInstanceId()) : 0;
        }

        public bool IsPieceValid(MapPiece piece)
        {
            return piece != null && GodotObject.IsInstanceValid(piece);
        }

        public IList<Edge> GetPieceEdges(MapPiece piece)
        {
            var result = new List<Edge>();
            if (piece == null)
            {
                return result;
            }

            var edges = piece.edges;
            for (int i = 0; i < edges.Length; i++)
            {
                result.Add(edges[i]);
            }

            return result;
        }

        public bool RemoveEdgeFromPiece(MapPiece piece, Edge edge)
        {
            if (piece == null || edge == null)
            {
                return false;
            }

            var edges = piece.edges;
            for (int i = edges.Length - 1; i >= 0; i--)
            {
                Edge currentEdge = edges[i];
                if (currentEdge != null && currentEdge.Matches(edge))
                {
                    piece.SetEdgeHasConnected(edge);
                    return true;
                }
            }

            return false;
        }

        public Vector2I GetPieceLogicalPos(MapPiece piece)
        {
            return piece != null ? piece.LogicalPos : Vector2I.Zero;
        }

        public int GetEdgeDir(Edge edge)
        {
            return edge != null ? (int)edge.Direction : 0;
        }

        public int GetEdgePos(Edge edge)
        {
            return edge != null ? (int)edge.Position : 0;
        }

        public Edge GetOppositeEdge(Edge edge)
        {
            return edge?.GetOpposite();
        }

        public bool EdgesMatch(Edge leftEdge, Edge rightEdge)
        {
            return leftEdge != null && rightEdge != null && leftEdge.Matches(rightEdge);
        }

        public bool PieceDataHasConnectingEdge(MapPieceData pieceData, Edge edge)
        {
            return pieceData != null && edge != null && pieceData.HasConnectingEdge(edge);
        }

        public bool PieceDataHasEdgeDir(MapPieceData pieceData, int dir)
        {
            return pieceData != null && pieceData.HasEdgeDir(dir);
        }

        public Vector2 GetPieceGlobalPosition(MapPiece piece)
        {
            return piece != null ? piece.GlobalPosition : Vector2.Zero;
        }

        public IList<Vector2> GetRouteWaypoints(MapPiece piece, int entryDir, int exitDir)
        {
            return piece != null ? piece.GetRouteWaypoints(entryDir, exitDir) : new List<Vector2>();
        }

        public IList<Vector2> GetFinalRouteWaypoints(MapPiece piece, int entryDir)
        {
            return piece != null ? piece.GetFinalRouteWaypoints(entryDir) : new List<Vector2>();
        }

        public int GetOppositeDir(int dir)
        {
            return (int)Edge.GetOppositeDir((Edge.Dir)dir);
        }

        public bool PieceDataIsFork(MapPieceData pieceData)
        {
            return pieceData != null && pieceData.IsFork;
        }
    }
    public override void _Ready()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        _mapPieces = dataLoader.GetAllMapPieces();

        _gridManager = new GridManager();
        _wordBuilderAdapter = new GodotWordBuilderAdapter();
        _connectionGraph = new PieceConnectionGraph(_wordBuilderAdapter);
        _frontierManager = new FrontierManager(_wordBuilderAdapter);
        _frontierManager.Setup(_gridManager, _mapPieces);
        _spawnHandler = new SpawnPositionsHandler();
        _spawnHandler.Setup(visual);

        if (_gridManager == null || _connectionGraph == null || _spawnHandler == null)
        {
            GD.PushError("[WorldMap] Failed to initialize world builder managers.");
            return;
        }

        _frontierManager.EdgeFinalized += OnEdgeFinalized;

        MapPieceData initPieceData = PickRandom(dataLoader.GetAllInitialMapPieces());
        MapPiece initPiece = initPieceData?.GetInstance();
        if (initPiece == null)
        {
            GD.PushError("[WorldMap] Could not instantiate initial piece.");
            return;
        }

        AddChild(initPiece);
        initPiece.LogicalPos = Vector2I.Zero;
        MovePieceDecorationToVisuals(initPiece);

        _gridManager.Occupy(Vector2I.Zero);
        _connectionGraph.RegisterPiece(initPiece);
        CompositeTileMap?.RegisterPiece(initPiece);
        _frontierManager.AddFrontier(initPiece);

        _routeBuilder = new RouteBuilder(_wordBuilderAdapter);
        _routeBuilder.Setup(_connectionGraph, initPiece);

        _lastPieceAttached = initPiece;
        UpdatePortals();
        AttachNextPiece();

        RunContext runContext = RunContext.Instance;
        _progress = runContext.Progress;
        _progress.CurrentWaveFinished += OnWaveFinished;
    }

    public override void _ExitTree()
    {
        if (_frontierManager != null)
        {
            _frontierManager.EdgeFinalized -= OnEdgeFinalized;
        }

        if (_progress != null)
        {
            _progress.CurrentWaveFinished -= OnWaveFinished;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("test"))
        {
            AttachNextPiece();
        }
    }

    public void AttachNextPiece()
    {
        if (!_frontierManager.HasFrontiers())
        {
            GD.PushError("No frontiers available for placement");
            return;
        }

        MapPiece frontier = _frontierManager.SelectRandomFrontier();
        if (frontier == null)
        {
            GD.PushError("Failed to select frontier");
            return;
        }

        var frontierEdges = frontier.edges;
        if (frontierEdges.Length == 0)
        {
            _frontierManager.RemoveFrontier(frontier);
            UpdatePortals();
            return;
        }

        Edge nextEdge = FrontierManagerPickRandomEdge(frontier);
        Vector2I frontierLogicalPos = frontier.LogicalPos;
        int nextEdgeDir = nextEdge != null ? (int)nextEdge.Direction : 0;
        Vector2I candidateTile = _gridManager.GetNeighborTile(frontierLogicalPos, nextEdgeDir);

        FrontierManager.EdgeValidationResult validation = _frontierManager.ValidateEdge(frontier, nextEdge, candidateTile);
        if (!validation.Valid)
        {
            string reason = string.IsNullOrEmpty(validation.Reason) ? "unknown reason" : validation.Reason;
            GD.PushWarning(reason);
            _frontierManager.RemoveEdgeFromFrontier(frontier, nextEdge);
            UpdatePortals();
            return;
        }

        List<MapPieceData> validPieces = validation.ValidPieces;
        Edge edgeToConnect = validation.EdgeToConnect;
        bool placed = TryPlaceOnEdge(frontier, nextEdge, candidateTile, validPieces, edgeToConnect);
        if (!placed)
        {
            GD.PushWarning($"frontier={frontier} edge={nextEdge} tile={candidateTile} no fitting piece -> removing edge");
            _frontierManager.RemoveEdgeFromFrontier(frontier, nextEdge);
            UpdatePortals();
            return;
        }

        _frontierManager.PruneAllFrontiers();
        UpdatePortals();
    }

    public List<Vector2> GetWaypointsForSpawnList(SpawnEntry spawnEntry)
    {
        if (_routeBuilder == null)
        {
            return new List<Vector2>();
        }

        return _routeBuilder.GetWaypointsForSpawn(spawnEntry);
    }

    public void UpdatePortals()
    {
        _portalEntries.Clear();

        for (int index = 0; index < _finalizedPortalEntries.Count; index++)
        {
            _portalEntries.Add(_finalizedPortalEntries[index]);
        }

        List<MapPiece> frontiers = _frontierManager.GetAllFrontiers();
        for (int index = 0; index < frontiers.Count; index++)
        {
            MapPiece frontier = frontiers[index];
            if (frontier == null)
            {
                continue;
            }

            var edges = frontier.edges;
            for (int edgeIndex = 0; edgeIndex < edges.Length; edgeIndex++)
            {
                Edge edge = edges[edgeIndex];
                int dir = edge != null ? (int)edge.Direction : 0;
                int pos = edge != null ? (int)edge.Position : 0;

                Vector2I logicalPos = frontier.LogicalPos;
                Vector2I tile = _gridManager.GetNeighborTile(logicalPos, dir);
                Vector2 worldPos = frontier.GlobalPosition
                    + PieceGetEdgeTilePos(frontier, dir, pos)
                    + PortalOffset;
                string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

                SpawnEntry entry = new()
                {
                    Key = key,
                    Tile = tile,
                    Position = worldPos,
                    Edge = edge,
                    Piece = frontier,
                };
                _portalEntries.Add(entry);
            }
        }

        if (_spawnHandler != null)
        {
            _spawnHandler.Update(_portalEntries);
        }
    }

    private void OnEdgeFinalized(MapPiece piece, Edge edge)
    {
        FinalizeSpawnPos(piece, edge);
    }

    private bool TryPlaceOnEdge(MapPiece frontier, Edge nextEdge, Vector2I candidateTile, List<MapPieceData> validPieces, Edge edgeToConnect)
    {
        List<MapPieceData> candidatePieces = BuildCandidatePieces(validPieces);
        Edge edgeToConnectTyped = edgeToConnect;

        for (int index = 0; index < candidatePieces.Count; index++)
        {
            MapPieceData pieceData = candidatePieces[index];
            MapPiece newPiece = pieceData?.GetInstance();
            if (newPiece == null)
            {
                continue;
            }

            AddChild(newPiece);

            HashSet<string> occSim = _gridManager.CreateSimulatedOccupation(candidateTile);
            var remainingEdges = new List<Edge>();
            for (int i = 0; i < newPiece.edges.Length; i++)
            {
                Edge edge = newPiece.edges[i];
                if (edge != null && (edgeToConnectTyped == null || !edge.Matches(edgeToConnectTyped)))
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

                int dir = (int)edgeObj.Direction;
                Vector2I neigh = candidateTile + _gridManager.GetOffset(dir);
                string neighKey = GridManager.VecKey(neigh);
                if (occSim.Contains(neighKey))
                {
                    continue;
                }

                if (_gridManager.ReachableToBoundary(neigh, occSim))
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

            newPiece.LogicalPos = candidateTile;
            _gridManager.Occupy(candidateTile);
            CompositeTileMap?.RegisterPiece(newPiece);

            PieceSetEdgeHasConnected(frontier, nextEdge);
            PieceSetEdgeHasConnected(newPiece, edgeToConnect);

            _frontierManager.UpdateAfterPlacement(frontier, newPiece);

            int entryDir = nextEdge != null ? (int)nextEdge.Direction : 0;
            int exitDir = edgeToConnectTyped != null ? (int)edgeToConnectTyped.Direction : 0;
            AttachPiece(frontier, newPiece, entryDir, exitDir);
            MovePieceDecorationToVisuals(newPiece);
            _lastPieceAttached = newPiece;
            if (!_hasPlacedFirstExpansion)
            {
                _hasPlacedFirstExpansion = true;
            }

            if (_pendingForkAfterBoss && pieceData != null && pieceData.IsFork)
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

        _connectionGraph.ConnectPieces(pieceA, pieceB, entryDir, exitDir);
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

        int childCount = node.GetChildCount();
        if (childCount == 0)
        {
            if (node is Node2D node2D)
            {
                output.Add(node2D);
            }

            return;
        }

        for (int index = 0; index < childCount; index++)
        {
            CollectLeafNode2d(node.GetChild(index), output);
        }
    }

    private void FinalizeSpawnPos(MapPiece piece, Edge edge)
    {
        if (piece == null)
        {
            return;
        }

        int dir = edge != null ? (int)edge.Direction : 0;
        int pos = edge != null ? (int)edge.Position : 0;
        Vector2I logicalPos = piece.LogicalPos;

        Vector2I tile = _gridManager.GetNeighborTile(logicalPos, dir);
        Vector2 position = piece.GlobalPosition
            + PieceGetEdgeTilePos(piece, dir, pos)
            + PortalOffset;
        string key = $"{logicalPos.X},{logicalPos.Y}_{dir}_{pos}";

        for (int index = 0; index < _finalizedPortalEntries.Count; index++)
        {
            SpawnEntry existing = _finalizedPortalEntries[index];
            if (existing != null && existing.Key == key)
            {
                return;
            }
        }

        SpawnEntry entry = new()
        {
            Key = key,
            Tile = tile,
            Position = position,
            Edge = edge,
            Piece = piece,
        };

        _finalizedPortalEntries.Add(entry);
    }

    private void OnWaveFinished()
    {
        RunContext runContext = RunContext.Instance;
        int currentWave = runContext.Progress.CurrentWave;

        if (!EnableFork && currentWave % WavesPerBoss == 0)
        {
            _pendingForkAfterBoss = true;
        }

        if (currentWave % 3 == 0)
        {
            AttachNextPiece();
        }
    }

    private List<MapPieceData> BuildCandidatePieces(List<MapPieceData> validPieces)
    {
        var candidatePieces = new List<MapPieceData>();

        if (!_hasPlacedFirstExpansion)
        {
            AppendFilteredByFork(validPieces, candidatePieces, false);
            Shuffle(candidatePieces);
            return candidatePieces;
        }

        if (EnableFork)
        {
            candidatePieces = new List<MapPieceData>(validPieces);
            Shuffle(candidatePieces);
            return candidatePieces;
        }

        if (_pendingForkAfterBoss)
        {
            var forkPieces = new List<MapPieceData>();
            var otherPieces = new List<MapPieceData>();
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
        return piece != null ? piece.GetEdgeTilePos(dir, pos) : Vector2.Zero;
    }

    private Vector2I PieceGetEdgeTileDelta(MapPiece piece, int dir)
    {
        return piece != null ? piece.GetEdgeTileDelta(dir) : Vector2I.Zero;
    }

    private Vector2 PieceGetTileLocalOffset(MapPiece piece, Vector2I delta)
    {
        return piece != null ? piece.GetTileLocalOffset(delta) : Vector2.Zero;
    }

    private Node2D PieceGetDecoration(MapPiece piece)
    {
        return piece?.GetDecoration();
    }

    private void PieceSetEdgeHasConnected(MapPiece piece, Edge edge)
    {
        piece?.SetEdgeHasConnected(edge);
    }

    private void AppendFilteredByFork(List<MapPieceData> source, List<MapPieceData> target, bool isFork)
    {
        for (int index = 0; index < source.Count; index++)
        {
            MapPieceData pieceData = source[index];
            if (pieceData != null && pieceData.IsFork == isFork)
            {
                target.Add(source[index]);
            }
        }
    }

    private T PickRandom<T>(List<T> items)
    {
        if (items.Count == 0)
        {
            return default;
        }

        int index = (int)(GD.Randi() % (uint)items.Count);
        return items[index];
    }

    private Edge FrontierManagerPickRandomEdge(MapPiece frontier)
    {
        return FrontierManager.PickRandomEdge(frontier, _wordBuilderAdapter);
    }

    private void Shuffle<T>(List<T> items)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}

