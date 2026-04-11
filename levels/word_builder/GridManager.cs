using Godot;

[GlobalClass]
public partial class GridManager : RefCounted
{
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;
    private const int EdgeDirSw = 2;
    private const int EdgeDirNw = 3;
    private static readonly Vector2I SentinelTile = new(-99999, -99999);

    public static readonly Godot.Collections.Dictionary GRID_OFFSETS = new()
    {
        { EdgeDirNe, new Vector2I(1, -1) },
        { EdgeDirSe, new Vector2I(1, 0) },
        { EdgeDirSw, new Vector2I(-1, 1) },
        { EdgeDirNw, new Vector2I(-1, 0) },
    };

    public static readonly Godot.Collections.Array<int> ALL_DIRS = new() { EdgeDirNe, EdgeDirSe, EdgeDirSw, EdgeDirNw };

    public Godot.Collections.Dictionary grid = new();

    public void occupy(Vector2I tile)
    {
        this.grid[tile] = true;
    }

    public bool is_occupied(Vector2I tile)
    {
        return this.grid.ContainsKey(tile);
    }

    public Vector2I get_neighbor_tile(Vector2I tile, int dir)
    {
        return tile + GRID_OFFSETS[dir].AsVector2I();
    }

    public Vector2I get_offset(int dir)
    {
        return GRID_OFFSETS[dir].AsVector2I();
    }

    public bool would_cause_enclosure_at(Vector2I candidate)
    {
        Godot.Collections.Dictionary simulated = this.grid.Duplicate();
        simulated[candidate] = true;

        for (int i = 0; i < ALL_DIRS.Count; i++)
        {
            int dir = ALL_DIRS[i];
            Vector2I n = candidate + GRID_OFFSETS[dir].AsVector2I();
            if (simulated.ContainsKey(n))
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

    public Godot.Collections.Array<int> get_invalid_edges_at(Vector2I tile, int dir_to_connect)
    {
        var invalidDirs = new Godot.Collections.Array<int>();

        for (int i = 0; i < ALL_DIRS.Count; i++)
        {
            int dir = ALL_DIRS[i];
            if (dir == dir_to_connect)
            {
                continue;
            }

            Vector2I newTile = tile + GRID_OFFSETS[dir].AsVector2I();
            if (this.grid.ContainsKey(newTile))
            {
                invalidDirs.Add(dir);
                continue;
            }

            Godot.Collections.Dictionary simulated = this.grid.Duplicate();
            simulated[newTile] = true;

            if (!FloodFill.can_escape_from(newTile, simulated, GRID_OFFSETS))
            {
                invalidDirs.Add(dir);
            }
        }

        return invalidDirs;
    }

    public bool reachable_to_boundary(Vector2I start, Godot.Collections.Dictionary occ, int lookahead = 8)
    {
        var xs = new Godot.Collections.Array<int>();
        var ys = new Godot.Collections.Array<int>();
        foreach (Variant key in occ.Keys)
        {
            string k = key.AsString();
            string[] parts = k.Split(',');
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
            minX = Mathf.Min(minX, xs[i]);
            maxX = Mathf.Max(maxX, xs[i]);
            minY = Mathf.Min(minY, ys[i]);
            maxY = Mathf.Max(maxY, ys[i]);
        }

        minX -= lookahead;
        maxX += lookahead;
        minY -= lookahead;
        maxY += lookahead;

        var q = new Godot.Collections.Array<Vector2I> { start };
        var seen = new Godot.Collections.Dictionary { { vec_key(start), true } };
        var neighs = new Godot.Collections.Array<Vector2I>
        {
            GRID_OFFSETS[EdgeDirNe].AsVector2I(),
            GRID_OFFSETS[EdgeDirSe].AsVector2I(),
            GRID_OFFSETS[EdgeDirSw].AsVector2I(),
            GRID_OFFSETS[EdgeDirNw].AsVector2I(),
        };

        while (q.Count > 0)
        {
            Vector2I cur = q[0];
            q.RemoveAt(0);

            if (cur.X <= minX || cur.X >= maxX || cur.Y <= minY || cur.Y >= maxY)
            {
                return true;
            }

            for (int i = 0; i < neighs.Count; i++)
            {
                Vector2I n = cur + neighs[i];
                string key = vec_key(n);
                if (seen.ContainsKey(key) || occ.ContainsKey(key))
                {
                    continue;
                }

                seen[key] = true;
                q.Add(n);
            }
        }

        return false;
    }

    public Godot.Collections.Dictionary create_simulated_occupation(Vector2I? extra_tile = null)
    {
        var occ = new Godot.Collections.Dictionary();
        foreach (Variant k in this.grid.Keys)
        {
            Vector2I tile = k.AsVector2I();
            occ[vec_key(tile)] = true;
        }

        Vector2I extra = extra_tile ?? SentinelTile;
        if (extra != SentinelTile)
        {
            occ[vec_key(extra)] = true;
        }

        return occ;
    }

    public static string vec_key(Vector2I v)
    {
        return $"{v.X},{v.Y}";
    }
}
