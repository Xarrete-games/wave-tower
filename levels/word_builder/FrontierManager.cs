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
        public List<object> ValidPieces { get; } = new();
        public object EdgeToConnect { get; set; }
    }

    public event Action<object, object> edge_finalized;

    private readonly IWordBuilderAdapter _adapter;
    private readonly List<object> _frontiers = new();
    private readonly List<object> _availablePieces = new();
    private GridManager _gridManager;

    public FrontierManager(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void setup(GridManager gridManager, IEnumerable<object> availablePieces)
    {
        _gridManager = gridManager;
        _frontiers.Clear();
        _availablePieces.Clear();
        _availablePieces.AddRange(availablePieces);
    }

    public void add_frontier(object piece)
    {
        if (!_adapter.IsPieceValid(piece))
        {
            return;
        }

        IList<object> edges = _adapter.GetPieceEdges(piece);
        if (edges.Count > 0 && !_frontiers.Contains(piece))
        {
            _frontiers.Add(piece);
        }
    }

    public void remove_frontier(object piece)
    {
        _frontiers.Remove(piece);
    }

    public bool has_frontiers()
    {
        return _frontiers.Count > 0;
    }

    public object select_random_frontier()
    {
        if (_frontiers.Count == 0)
        {
            return null;
        }

        int index = (int)(GD.Randi() % (uint)_frontiers.Count);
        return _frontiers[index];
    }

    public static object pick_random_edge(object frontier, IWordBuilderAdapter adapter)
    {
        if (!adapter.IsPieceValid(frontier))
        {
            return null;
        }

        IList<object> edgeList = adapter.GetPieceEdges(frontier);
        if (edgeList.Count == 0)
        {
            return null;
        }

        int index = (int)(GD.Randi() % (uint)edgeList.Count);
        return edgeList[index];
    }

    public EdgeValidationResult validate_edge(object frontier, object nextEdge, Vector2I candidateTile)
    {
        object edgeToConnect = _adapter.GetOppositeEdge(nextEdge);
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

        if (_gridManager.is_occupied(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=occupied";
            return result;
        }

        if (_gridManager.would_cause_enclosure_at(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=enclose";
            return result;
        }

        int dir = _adapter.GetEdgeDir(edgeToConnect);
        List<int> invalidEdges = _gridManager.get_invalid_edges_at(candidateTile, dir);
        result.InvalidEdges.AddRange(invalidEdges);

        for (int index = 0; index < _availablePieces.Count; index++)
        {
            object pieceData = _availablePieces[index];
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

    public void remove_edge_from_frontier(object frontier, object edge)
    {
        edge_finalized?.Invoke(frontier, edge);

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

    public void update_after_placement(object oldFrontier, object newPiece)
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

    public void prune_all_frontiers()
    {
        var removeFrontiers = new List<object>();

        for (int fi = 0; fi < _frontiers.Count; fi++)
        {
            object frontier = _frontiers[fi];
            if (!_adapter.IsPieceValid(frontier))
            {
                continue;
            }

            var removeEdges = new List<object>();
            var edgesCopy = new List<object>(_adapter.GetPieceEdges(frontier));

            for (int ei = 0; ei < edgesCopy.Count; ei++)
            {
                object edge = edgesCopy[ei];
                int dir = _adapter.GetEdgeDir(edge);
                Vector2I logicalPos = _adapter.GetPieceLogicalPos(frontier);
                Vector2I candidate = _gridManager.get_neighbor_tile(logicalPos, dir);

                if (_gridManager.is_occupied(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                if (_gridManager.would_cause_enclosure_at(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                object edgeToConnect = _adapter.GetOppositeEdge(edge);
                int edgeToConnectDir = _adapter.GetEdgeDir(edgeToConnect);
                List<int> invalid = _gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

                bool hasPossiblePiece = false;
                for (int pi = 0; pi < _availablePieces.Count; pi++)
                {
                    object pieceData = _availablePieces[pi];
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

            IList<object> frontierEdges = _adapter.GetPieceEdges(frontier);
            for (int ri = 0; ri < removeEdges.Count; ri++)
            {
                object removeEdge = removeEdges[ri];
                edge_finalized?.Invoke(frontier, removeEdge);
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

    public bool frontier_has_valid_edges(object piece)
    {
        if (!_adapter.IsPieceValid(piece))
        {
            return false;
        }

        IList<object> edges = _adapter.GetPieceEdges(piece);
        for (int ei = 0; ei < edges.Count; ei++)
        {
            object edge = edges[ei];
            int dir = _adapter.GetEdgeDir(edge);
            Vector2I logicalPos = _adapter.GetPieceLogicalPos(piece);
            Vector2I candidate = _gridManager.get_neighbor_tile(logicalPos, dir);

            if (_gridManager.is_occupied(candidate))
            {
                continue;
            }

            if (_gridManager.would_cause_enclosure_at(candidate))
            {
                continue;
            }

            object edgeToConnect = _adapter.GetOppositeEdge(edge);
            int edgeToConnectDir = _adapter.GetEdgeDir(edgeToConnect);
            List<int> invalid = _gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

            for (int pi = 0; pi < _availablePieces.Count; pi++)
            {
                object pieceData = _availablePieces[pi];
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

    public List<object> get_all_frontiers()
    {
        return new List<object>(_frontiers);
    }
}
