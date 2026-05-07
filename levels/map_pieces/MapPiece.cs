using Godot;
using System;
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

    public static readonly Vector2I Size = new(15, 15);
    private const string BuildableCustomDataKey = "buildeable";
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
    public Edge[] edges { get; set; } = Array.Empty<Edge>();

    [Export]
    public Vector2I LogicalPos { get; set; } = Vector2I.Zero;

    private readonly Dictionary<string, List<List<Vector2>>> _routeCache = new();

    private TileMapLayer _tileMap;
    private Node2D _decoration;

    public override void _Ready()
    {
        _tileMap = GetNodeOrNull<TileMapLayer>("MapPieceTileMap");
        _decoration = GetNodeOrNull<Node2D>("Decoration");
        PrecalculateRoutes();
    }

    public Node2D GetDecoration()
    {
        return _decoration;
    }

    public Vector2 GetEdgeTilePos(int dir, int pos = (int)DirPos.MIDDLE)
    {
        List<Vector2I> used = new();
        if (_tileMap != null)
        {
            foreach (Vector2I cell in _tileMap.GetUsedCells())
            {
                used.Add(cell);
            }
        }

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
        return MapTileToLocal(edgeTiles[tileIndex]);
    }

    public void SetEdgeHasConnected(Edge edge)
    {
        if (edges == null || edges.Length == 0)
        {
            GD.PushError($"Trying to set edge {edge} as connected, but this piece has no edges");
            return;
        }

        int removeIndex = -1;
        for (int i = 0; i < edges.Length; i++)
        {
            Edge e = edges[i];
            if (e != null && e.Matches(edge))
            {
                removeIndex = i;
                break;
            }
        }

        if (removeIndex < 0)
        {
            GD.PushError($"Trying to set edge {edge} as connected, but it is not an edge in this piece");
            return;
        }

        var updatedEdges = new List<Edge>(edges);
        updatedEdges.RemoveAt(removeIndex);
        edges = updatedEdges.ToArray();
    }

    public Edge FindEdgeByDir(int dir)
    {
        for (int i = 0; i < edges.Length; i++)
        {
            Edge e = edges[i];
            if (e != null && (int)e.Direction == dir)
            {
                return e;
            }
        }

        return null;
    }

    public bool HasEdgeDir(int dir)
    {
        return FindEdgeByDir(dir) != null;
    }

    public Vector2 MapTileToLocal(Vector2I tilePos)
    {
        Vector2 localInTilemap = _tileMap.MapToLocal(tilePos);
        Vector2 globalPoint = _tileMap.ToGlobal(localInTilemap);
        return ToLocal(globalPoint);
    }

    public Vector2 GetEdgeNormal(int dir)
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

    public Vector2I GetEdgeTileDelta(int dir)
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

    public Vector2 GetTileLocalOffset(Vector2I delta)
    {
        Vector2 origin = MapTileToLocal(Vector2I.Zero);
        Vector2 target = MapTileToLocal(delta);
        return target - origin;
    }

    public List<Vector2> GetRouteWaypoints(int entryDir, int exitDir)
    {
        string cacheKey = GetRouteCacheKey(entryDir, exitDir);
        if (!_routeCache.TryGetValue(cacheKey, out List<List<Vector2>> variants))
        {
            return new List<Vector2>();
        }

        if (variants.Count == 0)
        {
            return new List<Vector2>();
        }

        List<Vector2> cached = variants[(int)(GD.Randi() % (uint)variants.Count)];
        int canonicalFirst = Mathf.Min(entryDir, exitDir);
        var result = new List<Vector2>(cached.Count);

        if (entryDir != canonicalFirst)
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

    public void PrecalculateRoutes()
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

            var snappedPoints = new List<Vector2>();
            for (int i = 0; i < curve.PointCount; i++)
            {
                Vector2 pointLocal = path2d.Position + curve.GetPointPosition(i);
                Vector2 tileCenter = SnapToTileCenter(pointLocal);
                if (snappedPoints.Count == 0 || snappedPoints[snappedPoints.Count - 1] != tileCenter)
                {
                    snappedPoints.Add(tileCenter);
                }
            }

            string baseKey = GetRouteBaseKey(path2d.Name.ToString());
            if (!_routeCache.ContainsKey(baseKey))
            {
                _routeCache[baseKey] = new List<List<Vector2>>();
            }

            _routeCache[baseKey].Add(snappedPoints);
        }
    }

    public Vector2 SnapToTileCenter(Vector2 localPos)
    {
        Vector2 tilemapLocal = _tileMap.ToLocal(ToGlobal(localPos));
        Vector2I tileCoord = _tileMap.LocalToMap(tilemapLocal);
        return MapTileToLocal(tileCoord);
    }

    public string GetRouteCacheKey(int dirA, int dirB)
    {
        int first = Mathf.Min(dirA, dirB);
        int second = Mathf.Max(dirA, dirB);
        return $"route_{DIR_NAMES[first]}_{DIR_NAMES[second]}";
    }

    public string GetRouteBaseKey(string pathName)
    {
        Match match = Regex.Match(pathName, "^(route_[A-Z]+_[A-Z]+)(?:_\\d+)?$");
        return match.Success ? match.Groups[1].Value : pathName;
    }

    public void LimitBuildableTiles(int maxCount)
    {
        TileMapLayer tm = _tileMap;
        var buildableCoords = new List<Vector2I>();

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

            if (td.GetCustomData(BuildableCustomDataKey).AsBool())
            {
                buildableCoords.Add(coords);
            }
        }

        if (buildableCoords.Count <= maxCount)
        {
            return;
        }

        for (int index = buildableCoords.Count - 1; index > 0; index--)
        {
            int swapIndex = (int)(GD.Randi() % (uint)(index + 1));
            (buildableCoords[index], buildableCoords[swapIndex]) = (buildableCoords[swapIndex], buildableCoords[index]);
        }

        for (int i = maxCount; i < buildableCoords.Count; i++)
        {
            tm.SetCell(buildableCoords[i], ATLAS_ID, NORMAL_TILE_POS);
        }
    }

    public List<Vector2> GetFinalRouteWaypoints(int entryDir)
    {
        string cacheKey = $"route_{DIR_NAMES[entryDir]}_END";
        if (!_routeCache.TryGetValue(cacheKey, out List<List<Vector2>> variants))
        {
            return new List<Vector2>();
        }

        if (variants.Count == 0)
        {
            return new List<Vector2>();
        }

        List<Vector2> cached = variants[(int)(GD.Randi() % (uint)variants.Count)];
        var result = new List<Vector2>(cached.Count);
        for (int i = 0; i < cached.Count; i++)
        {
            result.Add(cached[i]);
        }

        return result;
    }

    private List<Vector2I> GetEdgeTilesByDir(List<Vector2I> used, int dir)
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

