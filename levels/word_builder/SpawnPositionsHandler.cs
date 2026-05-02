using Godot;
using System.Collections.Generic;

public class SpawnPositionsHandler
{
    private const string PortalGroup = "orange_portal";
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;

    private static readonly PackedScene OrangePortal = GD.Load<PackedScene>("uid://b8g0wp8j02vu4");

    private Node2D _portalsContainer;
    private Dictionary<string, SpawnEntry> _portalEntriesMap = new();
    private readonly List<Vector2> _portalPositions = new();
    private readonly Dictionary<string, Node2D> _portalNodes = new();

    public void Setup(Node2D portalsContainer)
    {
        _portalsContainer = portalsContainer;
    }

    public void Update(IReadOnlyList<SpawnEntry> entries)
    {
        var newMap = new Dictionary<string, SpawnEntry>();

        for (int index = 0; index < entries.Count; index++)
        {
            SpawnEntry entry = entries[index];
            if (entry == null)
            {
                continue;
            }

            string key = entry.Key;
            if (string.IsNullOrEmpty(key))
            {
                GD.PushError("[SpawnPositionsHandler] Spawn entry key is required.");
                continue;
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

        foreach (KeyValuePair<string, SpawnEntry> pair in newMap)
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

            SpawnEntry entry = pair.Value;
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

            portal.GlobalPosition = entry.Position;

            portal.AddToGroup(PortalGroup);

            int dir = entry.Dir;

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
        foreach (KeyValuePair<string, SpawnEntry> pair in _portalEntriesMap)
        {
            SpawnEntry entry = pair.Value;
            _portalPositions.Add(entry.Position);
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

}

