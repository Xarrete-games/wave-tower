using System.Collections.Generic;

public class PieceConnectionGraph
{
    private readonly IWordBuilderAdapter _adapter;
    private readonly Dictionary<long, Dictionary<int, long>> _connections = new();
    private readonly Dictionary<long, object> _objects = new();

    public PieceConnectionGraph(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void register_piece(object piece)
    {
        long key = _adapter.GetObjectKey(piece);
        if (key == 0)
        {
            return;
        }

        if (!_connections.ContainsKey(key))
        {
            _connections[key] = new Dictionary<int, long>();
        }

        _objects[key] = piece;
    }

    public void connect_pieces(object pieceA, object pieceB, int dirA, int dirB)
    {
        long keyA = _adapter.GetObjectKey(pieceA);
        long keyB = _adapter.GetObjectKey(pieceB);
        if (keyA == 0 || keyB == 0)
        {
            return;
        }

        register_piece(pieceA);
        register_piece(pieceB);

        _connections[keyA][dirA] = keyB;
        _connections[keyB][dirB] = keyA;
    }

    public int find_connection_dir(object fromPiece, object toPiece)
    {
        long fromKey = _adapter.GetObjectKey(fromPiece);
        long toKey = _adapter.GetObjectKey(toPiece);
        if (fromKey == 0 || toKey == 0 || !_connections.TryGetValue(fromKey, out Dictionary<int, long> pieceConnections))
        {
            return 0;
        }

        foreach (KeyValuePair<int, long> pair in pieceConnections)
        {
            if (pair.Value == toKey)
            {
                return pair.Key;
            }
        }

        return 0;
    }

    public List<object> find_path(object fromPiece, object toPiece)
    {
        long fromKey = _adapter.GetObjectKey(fromPiece);
        long toKey = _adapter.GetObjectKey(toPiece);
        var empty = new List<object>();

        if (fromKey == 0 || toKey == 0)
        {
            return empty;
        }

        if (fromKey == toKey)
        {
            if (_objects.TryGetValue(fromKey, out object startObject))
            {
                empty.Add(startObject);
            }
            return empty;
        }

        var queue = new Queue<long>();
        queue.Enqueue(fromKey);

        var cameFrom = new Dictionary<long, long>
        {
            [fromKey] = 0,
        };

        while (queue.Count > 0)
        {
            long current = queue.Dequeue();
            if (!_connections.TryGetValue(current, out Dictionary<int, long> pieceConnections))
            {
                continue;
            }

            foreach (KeyValuePair<int, long> pair in pieceConnections)
            {
                long neighbor = pair.Value;
                if (neighbor == 0 || cameFrom.ContainsKey(neighbor))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                if (neighbor == toKey)
                {
                    return ReconstructPath(cameFrom, toKey);
                }

                queue.Enqueue(neighbor);
            }
        }

        return empty;
    }

    private List<object> ReconstructPath(Dictionary<long, long> cameFrom, long end)
    {
        var path = new List<object>();
        long current = end;

        while (current != 0)
        {
            if (_objects.TryGetValue(current, out object piece))
            {
                path.Add(piece);
            }

            if (!cameFrom.TryGetValue(current, out long parent))
            {
                break;
            }

            current = parent;
        }

        path.Reverse();
        return path;
    }
}
