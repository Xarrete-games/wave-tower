using Godot;

public class PieceConnectionGraph
{
    public Godot.Collections.Dictionary connections = new();

    public void register_piece(Variant piece)
    {
        GodotObject pieceObj = piece.AsGodotObject();
        if (pieceObj == null)
        {
            return;
        }

        if (!this.connections.ContainsKey(pieceObj))
        {
            this.connections[pieceObj] = new Godot.Collections.Dictionary();
        }
    }

    public void connect_pieces(Variant piece_a, Variant piece_b, int dir_a, int dir_b)
    {
        GodotObject a = piece_a.AsGodotObject();
        GodotObject b = piece_b.AsGodotObject();
        if (a == null || b == null)
        {
            return;
        }

        if (!this.connections.ContainsKey(a))
        {
            this.connections[a] = new Godot.Collections.Dictionary();
        }

        if (!this.connections.ContainsKey(b))
        {
            this.connections[b] = new Godot.Collections.Dictionary();
        }

        Godot.Collections.Dictionary fromA = this.connections[a].AsGodotDictionary();
        Godot.Collections.Dictionary fromB = this.connections[b].AsGodotDictionary();
        fromA[dir_a] = b;
        fromB[dir_b] = a;
    }

    public Godot.Collections.Dictionary get_connections(Variant piece)
    {
        GodotObject pieceObj = piece.AsGodotObject();
        if (pieceObj != null && this.connections.ContainsKey(pieceObj))
        {
            return this.connections[pieceObj].AsGodotDictionary();
        }

        return new Godot.Collections.Dictionary();
    }

    public int find_connection_dir(Variant from_piece, Variant to_piece)
    {
        GodotObject from = from_piece.AsGodotObject();
        GodotObject to = to_piece.AsGodotObject();
        if (from == null || to == null || !this.connections.ContainsKey(from))
        {
            GD.PushWarning("[PieceConnectionGraph] from_piece has no registered connections");
            return 0;
        }

        Godot.Collections.Dictionary pieceConnections = this.connections[from].AsGodotDictionary();
        foreach (Variant key in pieceConnections.Keys)
        {
            if (ReferenceEquals(pieceConnections[key].AsGodotObject(), to))
            {
                return key.AsInt32();
            }
        }

        GD.PushWarning($"[PieceConnectionGraph] No connection found from {from} to {to}");
        return 0;
    }

    public Godot.Collections.Array<Variant> find_path(Variant from_piece, Variant to_piece)
    {
        GodotObject from = from_piece.AsGodotObject();
        GodotObject to = to_piece.AsGodotObject();
        var empty = new Godot.Collections.Array<Variant>();
        if (from == null || to == null)
        {
            return empty;
        }

        if (ReferenceEquals(from, to))
        {
            empty.Add(from);
            return empty;
        }

        var queue = new Godot.Collections.Array<GodotObject> { from };
        var cameFrom = new Godot.Collections.Dictionary { { from, default(Variant) } };

        while (queue.Count > 0)
        {
            GodotObject current = queue[0];
            queue.RemoveAt(0);

            if (!this.connections.ContainsKey(current))
            {
                continue;
            }

            Godot.Collections.Dictionary pieceConnections = this.connections[current].AsGodotDictionary();
            foreach (Variant dirKey in pieceConnections.Keys)
            {
                GodotObject neighbor = pieceConnections[dirKey].AsGodotObject();
                if (neighbor == null || !GodotObject.IsInstanceValid(neighbor))
                {
                    continue;
                }

                if (cameFrom.ContainsKey(neighbor))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                if (ReferenceEquals(neighbor, to))
                {
                    return this._reconstruct_path(cameFrom, to);
                }

                queue.Add(neighbor);
            }
        }

        GD.PushWarning($"[PieceConnectionGraph] No route found from {from} to {to}");
        return new Godot.Collections.Array<Variant>();
    }

    private Godot.Collections.Array<Variant> _reconstruct_path(Godot.Collections.Dictionary came_from, GodotObject end)
    {
        var path = new Godot.Collections.Array<Variant>();
        GodotObject current = end;

        while (current != null)
        {
            path.Add(current);
            if (came_from.ContainsKey(current))
            {
                current = came_from[current].AsGodotObject();
            }
            else
            {
                break;
            }
        }

        path.Reverse();
        return path;
    }
}
