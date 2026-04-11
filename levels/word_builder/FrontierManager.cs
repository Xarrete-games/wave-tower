using Godot;

[GlobalClass]
public partial class FrontierManager : RefCounted
{
    [Signal]
    public delegate void edge_finalizedEventHandler(Variant piece, Variant edge);

    private readonly Godot.Collections.Array<Variant> _frontiers = new();
    private GridManager _gridManager;
    private Godot.Collections.Array _availablePieces = new();

    public void setup(GridManager p_grid_manager, Variant p_available_pieces)
    {
        this._gridManager = p_grid_manager;
        this._availablePieces = p_available_pieces.AsGodotArray();
        this._frontiers.Clear();
    }

    public void add_frontier(Variant piece)
    {
        GodotObject pieceObj = piece.AsGodotObject();
        if (pieceObj == null)
        {
            return;
        }

        Godot.Collections.Array edges = pieceObj.Get("edges").AsGodotArray();
        if (edges.Count > 0 && !this._frontiers.Contains(piece))
        {
            this._frontiers.Add(piece);
        }
    }

    public void remove_frontier(Variant piece)
    {
        this._frontiers.Remove(piece);
    }

    public bool has_frontiers()
    {
        return this._frontiers.Count > 0;
    }

    public Variant select_random_frontier()
    {
        if (this._frontiers.Count == 0)
        {
            return default;
        }

        int index = (int)(GD.Randi() % (uint)this._frontiers.Count);
        return this._frontiers[index];
    }

    public static Variant pick_random_edge(Variant frontier)
    {
        GodotObject frontierObj = frontier.AsGodotObject();
        if (frontierObj == null)
        {
            return default;
        }

        Godot.Collections.Array edgeList = frontierObj.Get("edges").AsGodotArray().Duplicate();
        if (edgeList.Count == 0)
        {
            return default;
        }

        int index = (int)(GD.Randi() % (uint)edgeList.Count);
        return edgeList[index];
    }

    public Godot.Collections.Dictionary validate_edge(Variant frontier, Variant next_edge, Vector2I candidate_tile)
    {
        Variant edgeToConnect = next_edge.AsGodotObject()?.Call("get_opposite") ?? default;
        var result = new Godot.Collections.Dictionary
        {
            { "valid", false },
            { "reason", string.Empty },
            { "invalid_edges", new Godot.Collections.Array() },
            { "valid_pieces", new Godot.Collections.Array() },
            { "edge_to_connect", edgeToConnect },
        };

        if (this._gridManager == null)
        {
            result["reason"] = "grid_manager is null";
            return result;
        }

        if (this._gridManager.is_occupied(candidate_tile))
        {
            result["reason"] = $"frontier={frontier} edge={next_edge} tile={candidate_tile} reason=occupied";
            return result;
        }

        if (this._gridManager.would_cause_enclosure_at(candidate_tile))
        {
            result["reason"] = $"frontier={frontier} edge={next_edge} tile={candidate_tile} reason=enclose";
            return result;
        }

        int dir = edgeToConnect.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
        Godot.Collections.Array<int> invalidEdges = this._gridManager.get_invalid_edges_at(candidate_tile, dir);
        Godot.Collections.Array validPieces = new();

        for (int index = 0; index < this._availablePieces.Count; index++)
        {
            GodotObject pieceData = this._availablePieces[index].AsGodotObject();
            if (pieceData == null)
            {
                continue;
            }

            bool hasConnectingEdge = pieceData.Call("has_connecting_edge", next_edge).AsBool();
            if (!hasConnectingEdge)
            {
                continue;
            }

            bool blockedByInvalid = false;
            for (int i = 0; i < invalidEdges.Count; i++)
            {
                if (pieceData.Call("has_edge_dir", invalidEdges[i]).AsBool())
                {
                    blockedByInvalid = true;
                    break;
                }
            }

            if (!blockedByInvalid)
            {
                validPieces.Add(this._availablePieces[index]);
            }
        }

        if (validPieces.Count == 0)
        {
            result["reason"] = $"frontier={frontier} edge={next_edge} tile={candidate_tile} reason=invalid_edges {invalidEdges}";
            result["invalid_edges"] = invalidEdges;
            return result;
        }

        result["valid"] = true;
        result["invalid_edges"] = invalidEdges;
        result["valid_pieces"] = validPieces;
        return result;
    }

    public void remove_edge_from_frontier(Variant frontier, Variant edge)
    {
        EmitSignal(SignalName.edge_finalized, frontier, edge);

        GodotObject frontierObj = frontier.AsGodotObject();
        if (frontierObj == null)
        {
            return;
        }

        Godot.Collections.Array edges = frontierObj.Get("edges").AsGodotArray();
        for (int i = edges.Count - 1; i >= 0; i--)
        {
            if (edges[i].AsGodotObject()?.Call("matches", edge).AsBool() == true)
            {
                edges.RemoveAt(i);
                break;
            }
        }

        if (edges.Count == 0)
        {
            this._frontiers.Remove(frontier);
        }
    }

    public void update_after_placement(Variant old_frontier, Variant new_piece)
    {
        GodotObject newPieceObj = new_piece.AsGodotObject();
        GodotObject oldFrontierObj = old_frontier.AsGodotObject();
        if (newPieceObj == null || oldFrontierObj == null)
        {
            return;
        }

        if (newPieceObj.Get("edges").AsGodotArray().Count > 0)
        {
            this._frontiers.Add(new_piece);
        }

        if (oldFrontierObj.Get("edges").AsGodotArray().Count == 0)
        {
            this._frontiers.Remove(old_frontier);
        }
    }

    public void prune_all_frontiers()
    {
        Godot.Collections.Array removeFrontiers = new();

        for (int fi = 0; fi < this._frontiers.Count; fi++)
        {
            Variant frontier = this._frontiers[fi];
            GodotObject frontierObj = frontier.AsGodotObject();
            if (frontierObj == null)
            {
                continue;
            }

            Godot.Collections.Array removeEdges = new();
            Godot.Collections.Array edgesCopy = frontierObj.Get("edges").AsGodotArray().Duplicate();

            for (int ei = 0; ei < edgesCopy.Count; ei++)
            {
                Variant edge = edgesCopy[ei];
                int dir = edge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
                Vector2I logicalPos = frontierObj.Get("logical_pos").AsVector2I();
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

                Variant edgeToConnect = edge.AsGodotObject()?.Call("get_opposite") ?? default;
                int edgeToConnectDir = edgeToConnect.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
                Godot.Collections.Array<int> invalid = this._gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

                bool hasPossiblePiece = false;
                for (int pi = 0; pi < this._availablePieces.Count; pi++)
                {
                    GodotObject pieceData = this._availablePieces[pi].AsGodotObject();
                    if (pieceData == null || !pieceData.Call("has_connecting_edge", edge).AsBool())
                    {
                        continue;
                    }

                    bool blocked = false;
                    for (int ii = 0; ii < invalid.Count; ii++)
                    {
                        if (pieceData.Call("has_edge_dir", invalid[ii]).AsBool())
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

            Godot.Collections.Array frontierEdges = frontierObj.Get("edges").AsGodotArray();
            for (int ri = 0; ri < removeEdges.Count; ri++)
            {
                Variant removeEdge = removeEdges[ri];
                EmitSignal(SignalName.edge_finalized, frontier, removeEdge);
                for (int i = frontierEdges.Count - 1; i >= 0; i--)
                {
                    if (frontierEdges[i].AsGodotObject()?.Call("matches", removeEdge).AsBool() == true)
                    {
                        frontierEdges.RemoveAt(i);
                        break;
                    }
                }
            }

            if (frontierEdges.Count == 0)
            {
                removeFrontiers.Add(frontier);
            }
        }

        for (int index = 0; index < removeFrontiers.Count; index++)
        {
            this._frontiers.Remove(removeFrontiers[index]);
        }
    }

    public bool frontier_has_valid_edges(Variant piece)
    {
        GodotObject pieceObj = piece.AsGodotObject();
        if (pieceObj == null)
        {
            return false;
        }

        Godot.Collections.Array edges = pieceObj.Get("edges").AsGodotArray();
        for (int ei = 0; ei < edges.Count; ei++)
        {
            Variant edge = edges[ei];
            int dir = edge.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
            Vector2I logicalPos = pieceObj.Get("logical_pos").AsVector2I();
            Vector2I candidate = this._gridManager.get_neighbor_tile(logicalPos, dir);

            if (this._gridManager.is_occupied(candidate))
            {
                continue;
            }

            if (this._gridManager.would_cause_enclosure_at(candidate))
            {
                continue;
            }

            Variant edgeToConnect = edge.AsGodotObject()?.Call("get_opposite") ?? default;
            int edgeToConnectDir = edgeToConnect.AsGodotObject()?.Get("dir").AsInt32() ?? 0;
            Godot.Collections.Array<int> invalid = this._gridManager.get_invalid_edges_at(candidate, edgeToConnectDir);

            for (int pi = 0; pi < this._availablePieces.Count; pi++)
            {
                GodotObject pieceData = this._availablePieces[pi].AsGodotObject();
                if (pieceData == null || !pieceData.Call("has_connecting_edge", edge).AsBool())
                {
                    continue;
                }

                bool blocked = false;
                for (int ii = 0; ii < invalid.Count; ii++)
                {
                    if (pieceData.Call("has_edge_dir", invalid[ii]).AsBool())
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

    public Godot.Collections.Array<Variant> get_all_frontiers()
    {
        return this._frontiers.Duplicate();
    }
}
