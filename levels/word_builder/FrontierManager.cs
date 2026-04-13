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
        this._adapter = adapter;
    }

    public void setup(GridManager gridManager, IEnumerable<object> availablePieces)
    {
        this._gridManager = gridManager;
        this._frontiers.Clear();
        this._availablePieces.Clear();
        this._availablePieces.AddRange(availablePieces);
    }

    public void add_frontier(object piece)
    {
        if (!this._adapter.IsPieceValid(piece))
        {
            return;
        }

        IList<object> edges = this._adapter.GetPieceEdges(piece);
        if (edges.Count > 0 && !this._frontiers.Contains(piece))
        {
            this._frontiers.Add(piece);
        }
    }

    public void remove_frontier(object piece)
    {
        this._frontiers.Remove(piece);
    }

    public bool has_frontiers()
    {
        return this._frontiers.Count > 0;
    }

    public object select_random_frontier()
    {
        if (this._frontiers.Count == 0)
        {
            return null;
        }

        int index = (int)(GD.Randi() % (uint)this._frontiers.Count);
        return this._frontiers[index];
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
        object edgeToConnect = this._adapter.GetOppositeEdge(nextEdge);
        var result = new EdgeValidationResult
        {
            Valid = false,
            EdgeToConnect = edgeToConnect,
        };

        if (this._gridManager == null)
        {
            result.Reason = "grid_manager is null";
            return result;
        }

        if (this._gridManager.is_occupied(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=occupied";
            return result;
        }

        if (this._gridManager.would_cause_enclosure_at(candidateTile))
        {
            result.Reason = $"frontier={frontier} edge={nextEdge} tile={candidateTile} reason=enclose";
            return result;
        }

        int dir = this._adapter.GetEdgeDir(edgeToConnect);
        List<int> invalidEdges = this._gridManager.get_invalid_edges_at(candidateTile, dir);
        result.InvalidEdges.AddRange(invalidEdges);

        for (int index = 0; index < this._availablePieces.Count; index++)
        {
            object pieceData = this._availablePieces[index];
            if (!this._adapter.PieceDataHasConnectingEdge(pieceData, nextEdge))
            {
                continue;
            }

            bool blockedByInvalid = false;
            for (int i = 0; i < invalidEdges.Count; i++)
            {
                if (this._adapter.PieceDataHasEdgeDir(pieceData, invalidEdges[i]))
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
        this.edge_finalized?.Invoke(frontier, edge);

        if (!this._adapter.IsPieceValid(frontier))
        {
            return;
        }

        this._adapter.RemoveEdgeFromPiece(frontier, edge);

        if (this._adapter.GetPieceEdges(frontier).Count == 0)
        {
            this._frontiers.Remove(frontier);
        }
    }

    public void update_after_placement(object oldFrontier, object newPiece)
    {
        if (!this._adapter.IsPieceValid(newPiece) || !this._adapter.IsPieceValid(oldFrontier))
        {
            return;
        }

        if (this._adapter.GetPieceEdges(newPiece).Count > 0)
        {
            this._frontiers.Add(newPiece);
        }

        if (this._adapter.GetPieceEdges(oldFrontier).Count == 0)
        {
            this._frontiers.Remove(oldFrontier);
        }
    }

    public void prune_all_frontiers()
    {
        var removeFrontiers = new List<object>();

        for (int fi = 0; fi < this._frontiers.Count; fi++)
        {
            object frontier = this._frontiers[fi];
            if (!this._adapter.IsPieceValid(frontier))
            {
                continue;
            }

            var removeEdges = new List<object>();
            var edgesCopy = new List<object>(this._adapter.GetPieceEdges(frontier));

            for (int ei = 0; ei < edgesCopy.Count; ei++)
            {
                object edge = edgesCopy[ei];
                int dir = this._adapter.GetEdgeDir(edge);
                Vector2I logicalPos = this._adapter.GetPieceLogicalPos(frontier);
                Vector2I candidate = this._gridManager.get_neighbor_tile(logicalPos, dir);

                if (this._gridManager.is_occupied(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                if (this._gridManager.would_cause_enclosure_at(candidate))
                {
                    removeEdges.Add(edge);
                    continue;
                }

                object edgeToConnect = this._adapter.GetOppositeEdge(edge);
                int edgeToConnectDir = this._adapter.GetEdgeDir(edgeToConnect);
                List<int> invalid = this._gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

                bool hasPossiblePiece = false;
                for (int pi = 0; pi < this._availablePieces.Count; pi++)
                {
                    object pieceData = this._availablePieces[pi];
                    if (!this._adapter.PieceDataHasConnectingEdge(pieceData, edge))
                    {
                        continue;
                    }

                    bool blocked = false;
                    for (int ii = 0; ii < invalid.Count; ii++)
                    {
                        if (this._adapter.PieceDataHasEdgeDir(pieceData, invalid[ii]))
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

            IList<object> frontierEdges = this._adapter.GetPieceEdges(frontier);
            for (int ri = 0; ri < removeEdges.Count; ri++)
            {
                object removeEdge = removeEdges[ri];
                this.edge_finalized?.Invoke(frontier, removeEdge);
                this._adapter.RemoveEdgeFromPiece(frontier, removeEdge);
            }

            if (this._adapter.GetPieceEdges(frontier).Count == 0)
            {
                removeFrontiers.Add(frontier);
            }
        }

        for (int index = 0; index < removeFrontiers.Count; index++)
        {
            this._frontiers.Remove(removeFrontiers[index]);
        }
    }

    public bool frontier_has_valid_edges(object piece)
    {
        if (!this._adapter.IsPieceValid(piece))
        {
            return false;
        }

        IList<object> edges = this._adapter.GetPieceEdges(piece);
        for (int ei = 0; ei < edges.Count; ei++)
        {
            object edge = edges[ei];
            int dir = this._adapter.GetEdgeDir(edge);
            Vector2I logicalPos = this._adapter.GetPieceLogicalPos(piece);
            Vector2I candidate = this._gridManager.get_neighbor_tile(logicalPos, dir);

            if (this._gridManager.is_occupied(candidate))
            {
                continue;
            }

            if (this._gridManager.would_cause_enclosure_at(candidate))
            {
                continue;
            }

            object edgeToConnect = this._adapter.GetOppositeEdge(edge);
            int edgeToConnectDir = this._adapter.GetEdgeDir(edgeToConnect);
            List<int> invalid = this._gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

            for (int pi = 0; pi < this._availablePieces.Count; pi++)
            {
                object pieceData = this._availablePieces[pi];
                if (!this._adapter.PieceDataHasConnectingEdge(pieceData, edge))
                {
                    continue;
                }

                bool blocked = false;
                for (int ii = 0; ii < invalid.Count; ii++)
                {
                    if (this._adapter.PieceDataHasEdgeDir(pieceData, invalid[ii]))
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
        return new List<object>(this._frontiers);
    }
}
