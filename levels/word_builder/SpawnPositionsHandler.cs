using Godot;
using System.Collections.Generic;

public class SpawnPositionsHandler
{
    private const string PortalGroup = "orange_portal";
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;

    private static readonly PackedScene OrangePortal = GD.Load<PackedScene>("uid://b8g0wp8j02vu4");

    private Node2D _portalsContainer;
    private Dictionary<string, Dictionary<string, object>> _portalEntriesMap = new();
    private readonly List<Vector2> _portalPositions = new();
    private readonly Dictionary<string, Node2D> _portalNodes = new();

    public void Setup(Node2D portalsContainer)
    {
        _portalsContainer = portalsContainer;
    }

    public void Update(IReadOnlyList<Dictionary<string, object>> entries)
    {
        var newMap = new Dictionary<string, Dictionary<string, object>>();

        for (int index = 0; index < entries.Count; index++)
        {
            Dictionary<string, object> entry = entries[index];
            if (entry == null)
            {
                continue;
            }

            string key = string.Empty;
            if (entry.TryGetValue("key", out object keyObj) && keyObj is string rawKey)
            {
                key = rawKey;
            }
            else if (entry.TryGetValue("tile", out object tileObj) && tileObj is Vector2I tile)
            {
                key = TileKey(tile);
            }
            else
            {
                if (!entry.TryGetValue("pos", out object posObj) || posObj is not Vector2 pos)
                {
                    continue;
                }

                key = PosKey(pos);
            }

            newMap[key] = entry;
        }

        var toPurge = new List<string>();
        foreach (KeyValuePair<string, Node2D> pair in _portalNodes)
        {
            Node2D node = pair.Value;
            if (!GodotObject.IsInstanceValid(node))
            {
                toPurge.Add(pair.Key);
            }
        }

        for (int index = 0; index < toPurge.Count; index++)
        {
            _portalNodes.Remove(toPurge[index]);
        }

        var keysToRemove = new List<string>();
        foreach (KeyValuePair<string, Node2D> pair in _portalNodes)
        {
            if (!newMap.ContainsKey(pair.Key))
            {
                keysToRemove.Add(pair.Key);
            }
        }

        for (int index = 0; index < keysToRemove.Count; index++)
        {
            string key = keysToRemove[index];
            Node2D node = _portalNodes[key];
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }

            _portalNodes.Remove(key);
        }

        foreach (KeyValuePair<string, Dictionary<string, object>> pair in newMap)
        {
            string key = pair.Key;
            bool needsCreate = true;
            if (_portalNodes.TryGetValue(key, out Node2D existingPortal))
            {
                if (GodotObject.IsInstanceValid(existingPortal))
                {
                    needsCreate = false;
                }
                else
                {
                    _portalNodes.Remove(key);
                }
            }

            if (!needsCreate)
            {
                continue;
            }

            Dictionary<string, object> entry = pair.Value;
            Node2D portal = OrangePortal?.Instantiate() as Node2D;
            if (portal == null)
            {
                GD.PushError("[SpawnPositionsHandler] Orange portal scene is null or invalid");
                continue;
            }

            if (_portalsContainer != null)
            {
                _portalsContainer.AddChild(portal);
            }
            else
            {
                GD.PushError("[SpawnPositionsHandler]: No container to add portals to!");
            }

            if (entry.TryGetValue("pos", out object portalPosObj) && portalPosObj is Vector2 portalPos)
            {
                portal.GlobalPosition = portalPos;
            }

            portal.AddToGroup(PortalGroup);

            int dir = -1;
            if (entry.TryGetValue("dir", out object dirObj) && dirObj is int parsedDir)
            {
                dir = parsedDir;
            }
            else if (entry.TryGetValue("edge", out object edgeObj) && edgeObj is Edge edge)
            {
                dir = edge != null ? (int)edge.Direction : -1;
            }

            if (dir == EdgeDirNe || dir == EdgeDirSe)
            {
                AnimatedSprite2D sprite = portal.GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
                if (sprite != null)
                {
                    sprite.FlipH = true;
                }
                else
                {
                    foreach (Node child in portal.GetChildren())
                    {
                        if (child is AnimatedSprite2D animated)
                        {
                            animated.FlipH = true;
                            break;
                        }
                    }
                }
            }

            _portalNodes[key] = portal;
        }

        _portalEntriesMap = newMap;
        _portalPositions.Clear();
        foreach (KeyValuePair<string, Dictionary<string, object>> pair in _portalEntriesMap)
        {
            Dictionary<string, object> entry = pair.Value;
            if (entry.TryGetValue("pos", out object posObj) && posObj is Vector2 pos)
            {
                _portalPositions.Add(pos);
            }
        }
    }

    public List<Vector2> GetPositions()
    {
        return new List<Vector2>(_portalPositions);
    }

    public void ClearVisuals()
    {
        foreach (Node2D node in _portalNodes.Values)
        {
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }
        }

        _portalNodes.Clear();
    }

    private string PosKey(Vector2 pos)
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:F6},{1:F6}", pos.X, pos.Y);
    }

    private string TileKey(Vector2I tile)
    {
        return $"{tile.X},{tile.Y}";
    }
}

