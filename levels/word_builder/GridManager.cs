using Godot;
using System;
using System.Collections.Generic;

public class GridManager
{
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;
    private const int EdgeDirSw = 2;
    private const int EdgeDirNw = 3;
    private static readonly Vector2I SentinelTile = new(-99999, -99999);

    public static readonly Dictionary<int, Vector2I> GRID_OFFSETS = new()
    {
        { EdgeDirNe, new Vector2I(1, -1) },
        { EdgeDirSe, new Vector2I(1, 0) },
        { EdgeDirSw, new Vector2I(-1, 1) },
        { EdgeDirNw, new Vector2I(-1, 0) },
    };

    public static readonly int[] ALL_DIRS = { EdgeDirNe, EdgeDirSe, EdgeDirSw, EdgeDirNw };

    public readonly HashSet<Vector2I> grid = new();

    public void occupy(Vector2I tile)
    {
        this.grid.Add(tile);
    }

    public bool is_occupied(Vector2I tile)
    {
        return this.grid.Contains(tile);
    }

    public Vector2I get_neighbor_tile(Vector2I tile, int dir)
    {
        return tile + GRID_OFFSETS[dir];
    }

    public Vector2I get_offset(int dir)
    {
        return GRID_OFFSETS[dir];
    }

    public bool would_cause_enclosure_at(Vector2I candidate)
    {
        var simulated = new HashSet<Vector2I>(this.grid) { candidate };

        for (int i = 0; i < ALL_DIRS.Length; i++)
        {
            int dir = ALL_DIRS[i];
            Vector2I n = candidate + GRID_OFFSETS[dir];
            if (simulated.Contains(n))
            {
                continue;
            }

            if (FloodFill.can_escape_from(n, simulated, GRID_OFFSETS))
            {
                return false;
            }
        }

        return true;
    }

    public List<int> get_invalid_edges_at(Vector2I tile, int dir_to_connect)
    {
        var invalidDirs = new List<int>();

        for (int i = 0; i < ALL_DIRS.Length; i++)
        {
            int dir = ALL_DIRS[i];
            if (dir == dir_to_connect)
            {
                continue;
            }

            Vector2I newTile = tile + GRID_OFFSETS[dir];
            if (this.grid.Contains(newTile))
            {
                invalidDirs.Add(dir);
                continue;
            }

            var simulated = new HashSet<Vector2I>(this.grid) { newTile };

            if (!FloodFill.can_escape_from(newTile, simulated, GRID_OFFSETS))
            {
                invalidDirs.Add(dir);
            }
        }

        return invalidDirs;
    }

    public bool reachable_to_boundary(Vector2I start, HashSet<string> occ, int lookahead = 8)
    {
        var xs = new List<int>();
        var ys = new List<int>();
        foreach (string key in occ)
        {
            string[] parts = key.Split(',');
            if (parts.Length != 2)
            {
                continue;
            }

            xs.Add(int.Parse(parts[0]));
            ys.Add(int.Parse(parts[1]));
        }

        if (xs.Count == 0)
        {
            return true;
        }

        int minX = xs[0];
        int maxX = xs[0];
        int minY = ys[0];
        int maxY = ys[0];
        for (int i = 1; i < xs.Count; i++)
        {
            minX = Math.Min(minX, xs[i]);
            maxX = Math.Max(maxX, xs[i]);
            minY = Math.Min(minY, ys[i]);
            maxY = Math.Max(maxY, ys[i]);
        }

        minX -= lookahead;
        maxX += lookahead;
        minY -= lookahead;
        maxY += lookahead;

        var q = new Queue<Vector2I>();
        q.Enqueue(start);
        var seen = new HashSet<string> { vec_key(start) };
        var neighs = new Vector2I[]
        {
            GRID_OFFSETS[EdgeDirNe],
            GRID_OFFSETS[EdgeDirSe],
            GRID_OFFSETS[EdgeDirSw],
            GRID_OFFSETS[EdgeDirNw],
        };

        while (q.Count > 0)
        {
            Vector2I cur = q.Dequeue();

            if (cur.X <= minX || cur.X >= maxX || cur.Y <= minY || cur.Y >= maxY)
            {
                return true;
            }

            for (int i = 0; i < neighs.Length; i++)
            {
                Vector2I n = cur + neighs[i];
                string key = vec_key(n);
                if (seen.Contains(key) || occ.Contains(key))
                {
                    continue;
                }

                seen.Add(key);
                q.Enqueue(n);
            }
        }

        return false;
    }

    public HashSet<string> create_simulated_occupation(Vector2I? extra_tile = null)
    {
        var occ = new HashSet<string>();
        foreach (Vector2I tile in this.grid)
        {
            occ.Add(vec_key(tile));
        }

        Vector2I extra = extra_tile ?? SentinelTile;
        if (extra != SentinelTile)
        {
            occ.Add(vec_key(extra));
        }

        return occ;
    }

    public static string vec_key(Vector2I v)
    {
        return $"{v.X},{v.Y}";
    }
}
