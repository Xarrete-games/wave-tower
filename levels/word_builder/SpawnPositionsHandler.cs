using Godot;

public class SpawnPositionsHandler
{
    private const string PortalGroup = "orange_portal";
    private const int EdgeDirNe = 0;
    private const int EdgeDirSe = 1;

    private static readonly PackedScene OrangePortal = GD.Load<PackedScene>("uid://b8g0wp8j02vu4");

    private Node2D _portalsContainer;
    private readonly Godot.Collections.Dictionary<string, Godot.Collections.Dictionary> _portalEntriesMap = new();
    private readonly Godot.Collections.Array<Vector2> _portalPositions = new();
    private readonly Godot.Collections.Dictionary<string, Node> _portalNodes = new();

    public void setup(Node2D portals_container_p)
    {
        this._portalsContainer = portals_container_p;
    }

    public void update(Godot.Collections.Array<Godot.Collections.Dictionary> entries)
    {
        var newMap = new Godot.Collections.Dictionary<string, Godot.Collections.Dictionary>();

        for (int i = 0; i < entries.Count; i++)
        {
            Godot.Collections.Dictionary e = entries[i];
            if (e == null)
            {
                continue;
            }

            string key = string.Empty;
            if (e.ContainsKey("key"))
            {
                key = e["key"].AsString();
            }
            else if (e.ContainsKey("tile"))
            {
                key = this._tile_key(e["tile"].AsVector2I());
            }
            else
            {
                if (!e.ContainsKey("pos"))
                {
                    continue;
                }

                key = this._pos_key(e["pos"].AsVector2());
            }

            newMap[key] = e;
        }

        var toPurge = new Godot.Collections.Array<string>();
        foreach (string key in this._portalNodes.Keys)
        {
            Node node = this._portalNodes[key];
            if (!GodotObject.IsInstanceValid(node))
            {
                toPurge.Add(key);
            }
        }

        for (int i = 0; i < toPurge.Count; i++)
        {
            this._portalNodes.Remove(toPurge[i]);
        }

        var keysToRemove = new Godot.Collections.Array<string>();
        foreach (string key in this._portalNodes.Keys)
        {
            if (!newMap.ContainsKey(key))
            {
                keysToRemove.Add(key);
            }
        }

        for (int i = 0; i < keysToRemove.Count; i++)
        {
            string key = keysToRemove[i];
            Node node = this._portalNodes[key];
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }

            this._portalNodes.Remove(key);
        }

        foreach (string key in newMap.Keys)
        {
            bool needsCreate = true;
            if (this._portalNodes.ContainsKey(key))
            {
                Node existing = this._portalNodes[key];
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

            Godot.Collections.Dictionary e = newMap[key];
            Node portal = OrangePortal.Instantiate();
            if (this._portalsContainer != null)
            {
                this._portalsContainer.AddChild(portal);
            }
            else
            {
                GD.PushError("[SpawnPositionsHandler]: No container to add portals to!");
            }

            portal.Set("global_position", e["pos"]);
            portal.AddToGroup(PortalGroup);

            int dir = e.ContainsKey("dir") ? e["dir"].AsInt32() : -1;
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

        this._portalEntriesMap.Clear();
        foreach (string key in newMap.Keys)
        {
            this._portalEntriesMap[key] = newMap[key];
        }

        this._portalPositions.Clear();
        foreach (string key in this._portalEntriesMap.Keys)
        {
            this._portalPositions.Add(this._portalEntriesMap[key]["pos"].AsVector2());
        }
    }

    public Godot.Collections.Array<Vector2> get_positions()
    {
        return this._portalPositions.Duplicate();
    }

    public Godot.Collections.Array<Godot.Collections.Dictionary> get_entries()
    {
        var entries = new Godot.Collections.Array<Godot.Collections.Dictionary>();
        foreach (string key in this._portalEntriesMap.Keys)
        {
            entries.Add(this._portalEntriesMap[key]);
        }

        return entries;
    }

    public void clear_visuals()
    {
        foreach (string key in this._portalNodes.Keys)
        {
            Node node = this._portalNodes[key];
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
