using Godot;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[GlobalClass]
public partial class MapPiece : Node2D
{
    public enum Dir
    {
        NE,
        SE,
        SW,
        NW,
    }

    public enum DirPos
    {
        TOP,
        MIDDLE,
        BOTTOM,
    }

    public static readonly Vector2I size = new(15, 15);
    private const string BUILDEABLE_CUSTOM_DATA = "buildeable";
    private const string BLOCKED_CUSTOM_DATA = "blocked";
    private const int ATLAS_ID = 0;
    private static readonly Vector2I NORMAL_TILE_POS = new(2, 0);

    private static readonly Dictionary<int, string> DIR_NAMES = new()
    {
        { (int)Dir.NE, "NE" },
        { (int)Dir.SE, "SE" },
        { (int)Dir.SW, "SW" },
        { (int)Dir.NW, "NW" },
    };

    [Export]
    public Godot.Collections.Array<Edge> edges { get; set; } = new();

    [Export]
    public Vector2I logical_pos { get; set; } = Vector2I.Zero;

    private readonly Godot.Collections.Dictionary<string, Godot.Collections.Array<Godot.Collections.Array<Vector2>>> _routeCache = new();

    private TileMapLayer tile_map;
    private Node2D decoration;

    public override void _Ready()
    {
        tile_map = GetNodeOrNull<TileMapLayer>("MapPieceTileMap");
        decoration = GetNodeOrNull<Node2D>("Decoration");
        _precalculate_routes();
    }

    public Node2D get_decoration()
    {
        return decoration;
    }

    public Vector2 get_edge_tile_pos(int dir, int pos = (int)DirPos.MIDDLE)
    {
        Godot.Collections.Array<Vector2I> used = tile_map?.GetUsedCells() ?? new Godot.Collections.Array<Vector2I>();
        if (used.Count == 0)
        {
            GD.PushError("TileMap has no used cells");
            return Vector2.Zero;
        }

        List<Vector2I> edgeTiles = GetEdgeTilesByDir(used, dir);
        if (edgeTiles.Count == 0)
        {
            GD.PushError($"Edge has no tiles for dir {dir}");
            return Vector2.Zero;
        }

        int edgeSize = edgeTiles.Count;
        int tileIndex = pos switch
        {
            (int)DirPos.TOP => (int)(edgeSize * 0.25f),
            (int)DirPos.MIDDLE => (int)(edgeSize / 2.0f),
            (int)DirPos.BOTTOM => (int)(edgeSize * 0.75f),
            _ => (int)(edgeSize / 2.0f),
        };

        tileIndex = Mathf.Clamp(tileIndex, 0, edgeSize - 1);
        return _map_to_local(edgeTiles[tileIndex]);
    }

    public void set_edge_has_connected(Edge edge)
    {
        if (edges == null || edges.Count == 0)
        {
            GD.PushError($"Trying to set edge {edge} as connected, but this piece has no edges");
            return;
        }

        Edge toRemove = null;
        for (int i = 0; i < edges.Count; i++)
        {
            Edge e = edges[i];
            if (e != null && e.matches(edge))
            {
                toRemove = e;
                break;
            }
        }

        if (toRemove == null)
        {
            GD.PushError($"Trying to set edge {edge} as connected, but it is not an edge in this piece");
            return;
        }

        edges.Remove(toRemove);
    }

    public Edge find_edge_by_dir(int dir)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            Edge e = edges[i];
            if (e != null && (int)e.dir == dir)
            {
                return e;
            }
        }

        return null;
    }

    public bool has_edge_dir(int dir)
    {
        return find_edge_by_dir(dir) != null;
    }

    public Vector2 _map_to_local(Vector2I tile_pos)
    {
        Vector2 localInTilemap = tile_map.MapToLocal(tile_pos);
        Vector2 globalPoint = tile_map.ToGlobal(localInTilemap);
        return ToLocal(globalPoint);
    }

    public Vector2 get_edge_normal(int dir)
    {
        return dir switch
        {
            (int)Dir.NE => new Vector2(0, -1),
            (int)Dir.SE => new Vector2(1, 0),
            (int)Dir.SW => new Vector2(0, 1),
            (int)Dir.NW => new Vector2(-1, 0),
            _ => Vector2.Zero,
        };
    }

    public Vector2I get_edge_tile_delta(int dir)
    {
        return dir switch
        {
            (int)Dir.NE => new Vector2I(0, -1),
            (int)Dir.SE => new Vector2I(1, 0),
            (int)Dir.SW => new Vector2I(0, 1),
            (int)Dir.NW => new Vector2I(-1, 0),
            _ => Vector2I.Zero,
        };
    }

    public Vector2 get_tile_local_offset(Vector2I delta)
    {
        Vector2 origin = _map_to_local(Vector2I.Zero);
        Vector2 target = _map_to_local(delta);
        return target - origin;
    }

    public Godot.Collections.Array<Vector2> get_route_waypoints(int entry_dir, int exit_dir)
    {
        string cacheKey = _get_route_cache_key(entry_dir, exit_dir);
        if (!_routeCache.ContainsKey(cacheKey))
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Godot.Collections.Array<Godot.Collections.Array<Vector2>> variants = _routeCache[cacheKey];
        if (variants.Count == 0)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Godot.Collections.Array<Vector2> cached = variants[(int)(GD.Randi() % (uint)variants.Count)];
        int canonicalFirst = Mathf.Min(entry_dir, exit_dir);
        var result = new Godot.Collections.Array<Vector2>();

        if (entry_dir != canonicalFirst)
        {
            for (int i = cached.Count - 1; i >= 0; i--)
            {
                result.Add(cached[i]);
            }

            return result;
        }

        for (int i = 0; i < cached.Count; i++)
        {
            result.Add(cached[i]);
        }

        return result;
    }

    public void _precalculate_routes()
    {
        _routeCache.Clear();
        var pathNodes = new List<Path2D>();

        Node routesContainer = GetNodeOrNull<Node>("Routes");
        if (routesContainer != null)
        {
            foreach (Node child in routesContainer.GetChildren())
            {
                if (child is Path2D path && path.Name.ToString().StartsWith("route_"))
                {
                    pathNodes.Add(path);
                }
            }
        }

        foreach (Node child in GetChildren())
        {
            if (child is Path2D path && path.Name.ToString().StartsWith("route_"))
            {
                pathNodes.Add(path);
            }
        }

        for (int p = 0; p < pathNodes.Count; p++)
        {
            Path2D path2d = pathNodes[p];
            Curve2D curve = path2d.Curve;
            if (curve == null || curve.PointCount == 0)
            {
                continue;
            }

            var snappedPoints = new Godot.Collections.Array<Vector2>();
            for (int i = 0; i < curve.PointCount; i++)
            {
                Vector2 pointLocal = path2d.Position + curve.GetPointPosition(i);
                Vector2 tileCenter = _snap_to_tile_center(pointLocal);
                if (snappedPoints.Count == 0 || snappedPoints[snappedPoints.Count - 1] != tileCenter)
                {
                    snappedPoints.Add(tileCenter);
                }
            }

            string baseKey = _get_route_base_key(path2d.Name.ToString());
            if (!_routeCache.ContainsKey(baseKey))
            {
                _routeCache[baseKey] = new Godot.Collections.Array<Godot.Collections.Array<Vector2>>();
            }

            _routeCache[baseKey].Add(snappedPoints);
        }
    }

    public Vector2 _snap_to_tile_center(Vector2 local_pos)
    {
        Vector2 tilemapLocal = tile_map.ToLocal(ToGlobal(local_pos));
        Vector2I tileCoord = tile_map.LocalToMap(tilemapLocal);
        return _map_to_local(tileCoord);
    }

    public string _get_route_cache_key(int dir_a, int dir_b)
    {
        int first = Mathf.Min(dir_a, dir_b);
        int second = Mathf.Max(dir_a, dir_b);
        return $"route_{DIR_NAMES[first]}_{DIR_NAMES[second]}";
    }

    public string _get_route_base_key(string path_name)
    {
        Match match = Regex.Match(path_name, "^(route_[A-Z]+_[A-Z]+)(?:_\\d+)?$");
        return match.Success ? match.Groups[1].Value : path_name;
    }

    public void limit_buildeable_tiles(int max_count)
    {
        TileMapLayer tm = tile_map;
        var buildeableCoords = new Godot.Collections.Array<Vector2I>();

        foreach (Vector2I coords in tm.GetUsedCells())
        {
            TileData td = tm.GetCellTileData(coords);
            if (td == null)
            {
                continue;
            }

            if (td.GetCustomData(BLOCKED_CUSTOM_DATA).AsBool())
            {
                continue;
            }

            if (td.GetCustomData(BUILDEABLE_CUSTOM_DATA).AsBool())
            {
                buildeableCoords.Add(coords);
            }
        }

        if (buildeableCoords.Count <= max_count)
        {
            return;
        }

        buildeableCoords.Shuffle();
        for (int i = max_count; i < buildeableCoords.Count; i++)
        {
            tm.SetCell(buildeableCoords[i], ATLAS_ID, NORMAL_TILE_POS);
        }
    }

    public Godot.Collections.Array<Vector2> get_final_route_waypoints(int entry_dir)
    {
        string cacheKey = $"route_{DIR_NAMES[entry_dir]}_END";
        if (!_routeCache.ContainsKey(cacheKey))
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Godot.Collections.Array<Godot.Collections.Array<Vector2>> variants = _routeCache[cacheKey];
        if (variants.Count == 0)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        Godot.Collections.Array<Vector2> cached = variants[(int)(GD.Randi() % (uint)variants.Count)];
        var result = new Godot.Collections.Array<Vector2>();
        for (int i = 0; i < cached.Count; i++)
        {
            result.Add(cached[i]);
        }

        return result;
    }

    private List<Vector2I> GetEdgeTilesByDir(Godot.Collections.Array<Vector2I> used, int dir)
    {
        var result = new List<Vector2I>();
        if (used.Count == 0)
        {
            return result;
        }

        int minY = int.MaxValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int maxX = int.MinValue;
        for (int i = 0; i < used.Count; i++)
        {
            Vector2I c = used[i];
            minY = Mathf.Min(minY, c.Y);
            maxY = Mathf.Max(maxY, c.Y);
            minX = Mathf.Min(minX, c.X);
            maxX = Mathf.Max(maxX, c.X);
        }

        for (int i = 0; i < used.Count; i++)
        {
            Vector2I c = used[i];
            bool include = dir switch
            {
                (int)Dir.NE => c.Y == minY,
                (int)Dir.SW => c.Y == maxY,
                (int)Dir.NW => c.X == minX,
                (int)Dir.SE => c.X == maxX,
                _ => false,
            };

            if (include)
            {
                result.Add(c);
            }
        }

        if (dir == (int)Dir.NE || dir == (int)Dir.SW)
        {
            result.Sort((a, b) => a.X.CompareTo(b.X));
        }
        else
        {
            result.Sort((a, b) => a.Y.CompareTo(b.Y));
        }

        return result;
    }
}
