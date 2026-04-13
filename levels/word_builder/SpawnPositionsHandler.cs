using Godot;

public class SpawnPositionsHandler
{
    private const string PortalGroup = "orange_portal";
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;

    private static readonly PackedScene OrangePortal = GD.Load<PackedScene>("uid://b8g0wp8j02vu4");

    private Node2D _portalsContainer;
    private Godot.Collections.Dictionary _portalEntriesMap = new();
    private Godot.Collections.Array<Vector2> _portalPositions = new();
    private Godot.Collections.Dictionary _portalNodes = new();

    public void setup(Node2D portalsContainer)
    {
        this._portalsContainer = portalsContainer;
    }

    public void update(Godot.Collections.Array<Godot.Collections.Dictionary> entries)
    {
        var newMap = new Godot.Collections.Dictionary();

        for (int index = 0; index < entries.Count; index++)
        {
            Godot.Collections.Dictionary entry = entries[index];
            if (entry == null)
            {
                continue;
            }

            string key = string.Empty;
            if (entry.ContainsKey("key"))
            {
                key = entry["key"].AsString();
            }
            else if (entry.ContainsKey("tile"))
            {
                key = this._tile_key(entry["tile"].AsVector2I());
            }
            else
            {
                if (!entry.ContainsKey("pos"))
                {
                    continue;
                }

                key = this._pos_key(entry["pos"].AsVector2());
            }

            newMap[key] = entry;
        }

        var toPurge = new Godot.Collections.Array();
        foreach (Variant keyVar in this._portalNodes.Keys)
        {
            Variant nodeVariant = this._portalNodes[keyVar];
            Node node = nodeVariant.AsGodotObject() as Node;
            if (!GodotObject.IsInstanceValid(node))
            {
                toPurge.Add(keyVar);
            }
        }

        for (int i = 0; i < toPurge.Count; i++)
        {
            this._portalNodes.Remove(toPurge[i]);
        }

        var keysToRemove = new Godot.Collections.Array();
        foreach (Variant keyVar in this._portalNodes.Keys)
        {
            if (!newMap.ContainsKey(keyVar))
            {
                keysToRemove.Add(keyVar);
            }
        }

        for (int i = 0; i < keysToRemove.Count; i++)
        {
            Variant key = keysToRemove[i];
            Node node = this._portalNodes[key].AsGodotObject() as Node;
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }

            this._portalNodes.Remove(key);
        }

        foreach (Variant key in newMap.Keys)
        {
            bool needsCreate = true;
            if (this._portalNodes.ContainsKey(key))
            {
                Node existing = this._portalNodes[key].AsGodotObject() as Node;
                if (GodotObject.IsInstanceValid(existing))
                {
                    needsCreate = false;
                }
                else
                {
                    this._portalNodes.Remove(key);
                }
            }

            if (!needsCreate)
            {
                continue;
            }

            Godot.Collections.Dictionary entry = newMap[key].AsGodotDictionary();
            Node2D portal = OrangePortal?.Instantiate() as Node2D;
            if (portal == null)
            {
                GD.PushError("[SpawnPositionsHandler] Orange portal scene is null or invalid");
                continue;
            }

            if (this._portalsContainer != null)
            {
                this._portalsContainer.AddChild(portal);
            }
            else
            {
                GD.PushError("[SpawnPositionsHandler]: No container to add portals to!");
            }

            if (entry.ContainsKey("pos"))
            {
                portal.GlobalPosition = entry["pos"].AsVector2();
            }

            portal.AddToGroup(PortalGroup);

            int dir = -1;
            if (entry.ContainsKey("dir"))
            {
                dir = entry["dir"].AsInt32();
            }
            else if (entry.ContainsKey("edge"))
            {
                dir = entry["edge"].AsGodotObject()?.Get("dir").AsInt32() ?? -1;
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

            this._portalNodes[key] = portal;
        }

        this._portalEntriesMap = newMap;
        this._portalPositions.Clear();
        foreach (Variant key in this._portalEntriesMap.Keys)
        {
            Godot.Collections.Dictionary entry = this._portalEntriesMap[key].AsGodotDictionary();
            if (entry.ContainsKey("pos"))
            {
                this._portalPositions.Add(entry["pos"].AsVector2());
            }
        }
    }

    public Godot.Collections.Array<Vector2> get_positions()
    {
        return this._portalPositions.Duplicate();
    }

    public Godot.Collections.Array<Godot.Collections.Dictionary> get_entries()
    {
        var entries = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        foreach (Variant key in this._portalEntriesMap.Keys)
        {
            entries.Add(this._portalEntriesMap[key].AsGodotDictionary());
        }

        return entries;
    }

    public void clear_visuals()
    {
        foreach (Variant key in this._portalNodes.Keys)
        {
            Node node = this._portalNodes[key].AsGodotObject() as Node;
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }
        }

        this._portalNodes.Clear();
    }

    private string _pos_key(Vector2 pos)
    {
        return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:F6},{1:F6}", pos.X, pos.Y);
    }

    private string _tile_key(Vector2I tile)
    {
        return $"{tile.X},{tile.Y}";
    }
}
