using System;
using System.Collections.Generic;
using Godot;

public class FrontierManager
{
    public sealed class EdgeValidationResult
    {
        public bool Valid { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<int> InvalidEdges { get; } = new();
        public List<MapPieceData> ValidPieces { get; } = new();
        public Edge EdgeToConnect { get; set; }
    }

    public event Action<MapPiece, Edge> EdgeFinalized;

    private readonly IWordBuilderAdapter _adapter;
    private readonly List<MapPiece> _frontiers = new();
    private readonly List<MapPieceData> _availablePieces = new();
    private GridManager _gridManager;

    public FrontierManager(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void Setup(GridManager gridManager, IEnumerable<MapPieceData> availablePieces)
    {
        _gridManager = gridManager;
        _frontiers.Clear();
        _availablePieces.Clear();
        _availablePieces.AddRange(availablePieces);
    }

    public void AddFrontier(MapPiece piece)
    {
        if (!_adapter.IsPieceValid(piece))
        {
            return;
        }

        IList<Edge> edges = _adapter.GetPieceEdges(piece);
        if (edges.Count > 0 && !_frontiers.Contains(piece))
        {
            _frontiers.Add(piece);
        }
    }

    public void RemoveFrontier(MapPiece piece)
    {
        _frontiers.Remove(piece);
    }

    public bool HasFrontiers()
    {
        return _frontiers.Count > 0;
    }

    public MapPiece SelectRandomFrontier()
    {
        if (_frontiers.Count == 0)
        {
            return null;
        }

        int index = (int)(GD.Randi() % (uint)_frontiers.Count);
        return _frontiers[index];
    }

    public static Edge PickRandomEdge(MapPiece frontier, IWordBuilderAdapter adapter)
    {
        if (!adapter.IsPieceValid(frontier))
        {
            return null;
        }

        IList<Edge> edgeList = adapter.GetPieceEdges(frontier);
        if (edgeList.Count == 0)
        {
            return null;
        }

        int index = (int)(GD.Randi() % (uint)edgeList.Count);
        return edgeList[index];
    }

    public EdgeValidationResult ValidateEdge(MapPiece frontier, Edge nextEdge, Vector2I candidateTile)
    {
        Edge edgeToConnect = _adapter.GetOppositeEdge(nextEdge);
        var result = new EdgeValidationResult
        {
            Valid = false,
            EdgeToConnect = edgeToConnect,
        };

        if (_gridManager == null)
        {
            result.Reason = "grid_manager is null";
            return result;
        }

        if (_gridManager.IsOccupied(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=occupied";
            return result;
        }

        if (_gridManager.WouldCauseEnclosureAt(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=enclose";
            return result;
        }

        int dir = _adapter.GetEdgeDir(edgeToConnect);
        List<int> invalidEdges = _gridManager.GetInvalidEdgesAt(candidateTile, dir);
        result.InvalidEdges.AddRange(invalidEdges);

        for (int index = 0; index < _availablePieces.Count; index++)
        {
            MapPieceData pieceData = _availablePieces[index];
            if (!_adapter.PieceDataHasConnectingEdge(pieceData, nextEdge))
            {
                continue;
            }

            bool blockedByInvalid = false;
            for (int i = 0; i < invalidEdges.Count; i++)
            {
                if (_adapter.PieceDataHasEdgeDir(pieceData, invalidEdges[i]))
                {
                    blockedByInvalid = true;
                    break;
                }
            }

            if (!blockedByInvalid)
            {
                result.ValidPieces.Add(pieceData);
            }
        }

        if (result.ValidPieces.Count == 0)
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=invalid_edges";
            return result;
        }

        result.Valid = true;
        return result;
    }

    public void RemoveEdgeFromFrontier(MapPiece frontier, Edge edge)
    {
        EdgeFinalized?.Invoke(frontier, edge);

        if (!_adapter.IsPieceValid(frontier))
        {
            return;
        }

        _adapter.RemoveEdgeFromPiece(frontier, edge);

        if (_adapter.GetPieceEdges(frontier).Count == 0)
        {
            _frontiers.Remove(frontier);
        }
    }

    public void UpdateAfterPlacement(MapPiece oldFrontier, MapPiece newPiece)
    {
        if (!_adapter.IsPieceValid(newPiece) || !_adapter.IsPieceValid(oldFrontier))
        {
            return;
        }

        if (_adapter.GetPieceEdges(newPiece).Count > 0)
        {
            _frontiers.Add(newPiece);
        }

        if (_adapter.GetPieceEdges(oldFrontier).Count == 0)
        {
            _frontiers.Remove(oldFrontier);
        }
    }

    public void PruneAllFrontiers()
    {
        var removeFrontiers = new List<MapPiece>();

        for (int fi = 0; fi < _frontiers.Count; fi++)
        {
            MapPiece frontier = _frontiers[fi];
            if (!_adapter.IsPieceValid(frontier))
            {
                continue;
            }

            var removeEdges = new List<Edge>();
            var edgesCopy = new List<Edge>(_adapter.GetPieceEdges(frontier));

            for (int ei = 0; ei < edgesCopy.Count; ei++)
            {
                Edge edge = edgesCopy[ei];
                int dir = _adapter.GetEdgeDir(edge);
                Vector2I logicalPos = _adapter.GetPieceLogicalPos(frontier);
                Vector2I candidate = _gridManager.GetNeighborTile(logicalPos, dir);

                if (_gridManager.IsOccupied(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                if (_gridManager.WouldCauseEnclosureAt(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                Edge edgeToConnect = _adapter.GetOppositeEdge(edge);
                int edgeToConnectDir = _adapter.GetEdgeDir(edgeToConnect);
                List<int> invalid = _gridManager.GetInvalidEdgesAt(candidate, edgeToConnectDir);

                bool hasPossiblePiece = false;
                for (int pi = 0; pi < _availablePieces.Count; pi++)
                {
                    MapPieceData pieceData = _availablePieces[pi];
                    if (!_adapter.PieceDataHasConnectingEdge(pieceData, edge))
                    {
                        continue;
                    }

                    bool blocked = false;
                    for (int ii = 0; ii < invalid.Count; ii++)
                    {
                        if (_adapter.PieceDataHasEdgeDir(pieceData, invalid[ii]))
                        {
                            blocked = true;
                            break;
                        }
                    }

                    if (!blocked)
                    {
                        hasPossiblePiece = true;
                        break;
                    }
                }

                if (!hasPossiblePiece)
                {
                    removeEdges.Add(edge);
                }
            }

            for (int ri = 0; ri < removeEdges.Count; ri++)
            {
                Edge removeEdge = removeEdges[ri];
                EdgeFinalized?.Invoke(frontier, removeEdge);
                _adapter.RemoveEdgeFromPiece(frontier, removeEdge);
            }

            if (_adapter.GetPieceEdges(frontier).Count == 0)
            {
                removeFrontiers.Add(frontier);
            }
        }

        for (int index = 0; index < removeFrontiers.Count; index++)
        {
            _frontiers.Remove(removeFrontiers[index]);
        }
    }

    public bool FrontierHasValidEdges(MapPiece piece)
    {
        if (!_adapter.IsPieceValid(piece))
        {
            return false;
        }

        IList<Edge> edges = _adapter.GetPieceEdges(piece);
        for (int ei = 0; ei < edges.Count; ei++)
        {
            Edge edge = edges[ei];
            int dir = _adapter.GetEdgeDir(edge);
            Vector2I logicalPos = _adapter.GetPieceLogicalPos(piece);
            Vector2I candidate = _gridManager.GetNeighborTile(logicalPos, dir);

            if (_gridManager.IsOccupied(candidate))
            {
                continue;
            }

            if (_gridManager.WouldCauseEnclosureAt(candidate))
            {
                continue;
            }

            Edge edgeToConnect = _adapter.GetOppositeEdge(edge);
            int edgeToConnectDir = _adapter.GetEdgeDir(edgeToConnect);
            List<int> invalid = _gridManager.GetInvalidEdgesAt(candidate, edgeToConnectDir);

            for (int pi = 0; pi < _availablePieces.Count; pi++)
            {
                MapPieceData pieceData = _availablePieces[pi];
                if (!_adapter.PieceDataHasConnectingEdge(pieceData, edge))
                {
                    continue;
                }

                bool blocked = false;
                for (int ii = 0; ii < invalid.Count; ii++)
                {
                    if (_adapter.PieceDataHasEdgeDir(pieceData, invalid[ii]))
                    {
                        blocked = true;
                        break;
                    }
                }

                if (!blocked)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<MapPiece> GetAllFrontiers()
    {
        return new List<MapPiece>(_frontiers);
    }
}
