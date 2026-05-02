using System.Collections.Generic;

public class PieceConnectionGraph
{
    private readonly IWordBuilderAdapter _adapter;
    private readonly Dictionary<long, Dictionary<int, long>> _connections = new();
    private readonly Dictionary<long, MapPiece> _objects = new();

    public PieceConnectionGraph(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void RegisterPiece(MapPiece piece)
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

    public void ConnectPieces(MapPiece pieceA, MapPiece pieceB, int dirA, int dirB)
    {
        long keyA = _adapter.GetObjectKey(pieceA);
        long keyB = _adapter.GetObjectKey(pieceB);
        if (keyA == 0 || keyB == 0)
        {
            return;
        }

        RegisterPiece(pieceA);
        RegisterPiece(pieceB);

        _connections[keyA][dirA] = keyB;
        _connections[keyB][dirB] = keyA;
    }

    public int FindConnectionDir(MapPiece fromPiece, MapPiece toPiece)
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

    public List<MapPiece> FindPath(MapPiece fromPiece, MapPiece toPiece)
    {
        long fromKey = _adapter.GetObjectKey(fromPiece);
        long toKey = _adapter.GetObjectKey(toPiece);
        var empty = new List<MapPiece>();

        if (fromKey == 0 || toKey == 0)
        {
            return empty;
        }

        if (fromKey == toKey)
        {
            if (_objects.TryGetValue(fromKey, out MapPiece startObject))
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

    private List<MapPiece> ReconstructPath(Dictionary<long, long> cameFrom, long end)
    {
        var path = new List<MapPiece>();
        long current = end;

        while (current != 0)
        {
            if (_objects.TryGetValue(current, out MapPiece piece))
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
