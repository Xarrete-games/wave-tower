using Godot;

public partial class DataLoader : Node
{
    public static DataLoader Instance { get; private set; }

    private const string RELICS_DATA_PATH = "res://relics/data/";
    private const string EVENTS_DATA_PATH = "res://events/data/";
    private const string CONSUMABLES_DATA_PATH = "res://consumables/data/";
    private const string ENEMY_DEBUFFS_DATA_PATH = "res://enemies/enemy_debuff/data/";
    private const string TOWER_BUFFS_DATA_PATH = "res://towers/tower-buffs/data/";
    private const string INITIAL_MAP_PIECES_DATA_PATH = "res://levels/map_pieces/init/";
    private const string MAP_PIECES_DATA_PATH = "res://levels/map_pieces/data/";
    private const string TOWER_DATA_PATH = "res://towers/data/";

    public Godot.Collections.Array<Variant> relics = new();
    public Godot.Collections.Array<Variant> events = new();
    public Godot.Collections.Array<Variant> consumables = new();
    public Godot.Collections.Array<Variant> enemy_debuffs = new();
    public Godot.Collections.Array<Variant> tower_buffs_data = new();
    public Godot.Collections.Array<Variant> initial_map_pieces = new();
    public Godot.Collections.Array<Variant> map_pieces = new();
    public Godot.Collections.Array<Variant> tower_data = new();
    public GodotObject enemy_data;

    public override void _Ready()
    {
        Instance = this;

        Script enemyDataLoaderScript = GD.Load<Script>("res://global/enemy_data_loader.gd");
        this.enemy_data = enemyDataLoaderScript?.Call("new").AsGodotObject();

        this._load_relics();
        this._load_events();
        this._load_consumables();
        this._load_enemy_debuffs();
        this._load_tower_buffs_data();
        this._load_map_pieces();
        this._load__initial_map_pieces();
        this._load_tower_data();
    }

    public override void _ExitTree()
    {
        if (ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }

    public Variant get_relic_by_id(string relic_id)
    {
        for (int index = 0; index < this.relics.Count; index++)
        {
            GodotObject relic = this.relics[index].AsGodotObject();
            if (relic != null && this._has_property(relic, "id") && (string)relic.Get("id") == relic_id)
            {
                return this._duplicate_resource(this.relics[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_relics()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.relics, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_random_relics(int amount, Variant rarity = default, bool include_cursed = false, bool include_only_for_events = false)
    {
        var candidates = this.get_not_used_relics(rarity);
        var filtered = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < candidates.Count; index++)
        {
            GodotObject relicData = candidates[index].AsGodotObject();
            if (relicData == null)
            {
                continue;
            }

            bool isCursed = this._has_property(relicData, "is_cursed") && (bool)relicData.Get("is_cursed");
            bool onlyForEvents = this._has_property(relicData, "only_for_events") && (bool)relicData.Get("only_for_events");
            if (!include_cursed && isCursed)
            {
                continue;
            }

            if (!include_only_for_events && onlyForEvents)
            {
                continue;
            }

            filtered.Add(candidates[index]);
        }

        filtered.Shuffle();

        var result = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < filtered.Count && result.Count < amount; index++)
        {
            result.Add(filtered[index]);
        }

        return result;
    }

    public Godot.Collections.Array<Variant> get_not_used_relics(Variant rarity = default, Variant is_cursed = default)
    {
        var filtered = new Godot.Collections.Array<Variant>();
        Node runContext = GetNodeOrNull<Node>("/root/RunContext");
        GodotObject relicsManager = runContext?.Get("relics_manager").AsGodotObject();

        for (int index = 0; index < this.relics.Count; index++)
        {
            GodotObject relicData = this.relics[index].AsGodotObject();
            if (relicData == null || !this._has_property(relicData, "id"))
            {
                continue;
            }

            if (rarity.VariantType != Variant.Type.Nil)
            {
                if (!this._has_property(relicData, "rarity") || (int)relicData.Get("rarity") != (int)rarity)
                {
                    continue;
                }
            }

            if (is_cursed.VariantType != Variant.Type.Nil)
            {
                if (!this._has_property(relicData, "is_cursed"))
                {
                    continue;
                }

                bool relicIsCursed = (bool)relicData.Get("is_cursed");
                bool filterCursed = (bool)is_cursed;
                if (!filterCursed && relicIsCursed)
                {
                    continue;
                }

                if (filterCursed && !relicIsCursed)
                {
                    continue;
                }
            }

            bool alreadyOwned = false;
            if (relicsManager != null)
            {
                alreadyOwned = (bool)relicsManager.Call("has_relic", relicData.Get("id"));
            }

            if (!alreadyOwned)
            {
                filtered.Add(this.relics[index]);
            }
        }

        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(filtered, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_random_available_relics(int amount, Variant rarity = default, bool include_cursed = false, bool include_only_for_events = false)
    {
        var candidates = this.get_not_used_relics(rarity);
        var filtered = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < candidates.Count; index++)
        {
            GodotObject relicData = candidates[index].AsGodotObject();
            if (relicData == null)
            {
                continue;
            }

            if (!this._has_property(relicData, "is_cursed") || !this._has_property(relicData, "only_for_events"))
            {
                continue;
            }

            bool isCursed = (bool)relicData.Get("is_cursed");
            bool onlyForEvents = (bool)relicData.Get("only_for_events");
            if (!include_cursed && isCursed)
            {
                continue;
            }

            if (!include_only_for_events && onlyForEvents)
            {
                continue;
            }

            filtered.Add(candidates[index]);
        }

        filtered.Shuffle();

        var result = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < filtered.Count && result.Count < amount; index++)
        {
            result.Add(filtered[index]);
        }

        return result;
    }

    public Godot.Collections.Array<Variant> get_all_events()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.events, result);
        return result;
    }

    public Variant get_consumable_by_id(string consumable_id)
    {
        for (int index = 0; index < this.consumables.Count; index++)
        {
            GodotObject consumable = this.consumables[index].AsGodotObject();
            if (consumable != null && (string)consumable.Get("id") == consumable_id)
            {
                return this._duplicate_resource(this.consumables[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_consumables()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.consumables, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_consumables_of_type(int consumable_type)
    {
        var filtered = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < this.consumables.Count; index++)
        {
            GodotObject data = this.consumables[index].AsGodotObject();
            if (data != null && (int)data.Get("consumable_type") == consumable_type)
            {
                filtered.Add(this.consumables[index]);
            }
        }

        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(filtered, result);
        return result;
    }

    public Variant get_debuff_data(int type)
    {
        for (int index = 0; index < this.enemy_debuffs.Count; index++)
        {
            GodotObject debuffData = this.enemy_debuffs[index].AsGodotObject();
            if (debuffData != null && (int)debuffData.Get("debuff_type") == type)
            {
                return this._duplicate_resource(this.enemy_debuffs[index]);
            }
        }

        return default;
    }

    public Variant get_tower_buff_data_by_id(string buff_id)
    {
        for (int index = 0; index < this.tower_buffs_data.Count; index++)
        {
            GodotObject buffData = this.tower_buffs_data[index].AsGodotObject();
            if (buffData != null && (string)buffData.Get("id") == buff_id)
            {
                return this._duplicate_resource(this.tower_buffs_data[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_initial_map_pieces()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.initial_map_pieces, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_map_pieces()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.map_pieces, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_enemies()
    {
        if (this.enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return this.enemy_data.Call("get_all_enemies").AsGodotArray<Variant>();
    }

    public Godot.Collections.Array<Variant> get_enemies_by_type(int type)
    {
        if (this.enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return this.enemy_data.Call("get_enemies_by_type", type).AsGodotArray<Variant>();
    }

    public Godot.Collections.Array<Variant> get_spawnable_enemies()
    {
        if (this.enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return this.enemy_data.Call("get_spawnable_enemies").AsGodotArray<Variant>();
    }

    public Godot.Collections.Array<Variant> get_all_tower_data()
    {
        var result = new Godot.Collections.Array<Variant>();
        this._append_deep_copies(this.tower_data, result);
        return result;
    }

    private void _load_relics()
    {
        var loadedArray = this._load_resources_from_dir(RELICS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && this._has_property(data, "id") && this._has_property(data, "runtime_script"))
            {
                this.relics.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded relic has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void _load_events()
    {
        var loadedArray = this._load_resources_from_dir(EVENTS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            this.events.Add(loadedArray[index]);
        }
    }

    private void _load_consumables()
    {
        var loadedArray = this._load_resources_from_dir(CONSUMABLES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && this._has_property(data, "id") && this._has_property(data, "consumable_type"))
            {
                this.consumables.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded consumables data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void _load_enemy_debuffs()
    {
        var loadedArray = this._load_resources_from_dir(ENEMY_DEBUFFS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && this._has_property(data, "id") && this._has_property(data, "debuff_type"))
            {
                this.enemy_debuffs.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded enemy debuff data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void _load_tower_buffs_data()
    {
        var loadedArray = this._load_resources_from_dir(TOWER_BUFFS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && this._has_property(data, "id") && this._has_property(data, "runtime_script"))
            {
                this.tower_buffs_data.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded tower buff data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void _load_map_pieces()
    {
        var loadedArray = this._load_resources_from_dir(MAP_PIECES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            this.map_pieces.Add(loadedArray[index]);
        }
    }

    private void _load__initial_map_pieces()
    {
        var loadedArray = this._load_resources_from_dir(INITIAL_MAP_PIECES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            this.initial_map_pieces.Add(loadedArray[index]);
        }
    }

    private void _load_tower_data()
    {
        var loadedArray = this._load_resources_from_dir(TOWER_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && this._has_property(data, "data") && this._has_property(data, "scene"))
            {
                this.tower_data.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded tower data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private bool _has_property(GodotObject target, string property_name)
    {
        if (target == null)
        {
            return false;
        }

        var properties = target.GetPropertyList();
        for (int index = 0; index < properties.Count; index++)
        {
            var dict = properties[index];
            if (dict.ContainsKey("name") && dict["name"].AsString() == property_name)
            {
                return true;
            }
        }

        return false;
    }

    private Godot.Collections.Array<Variant> _load_resources_from_dir(string path)
    {
        var result = new Godot.Collections.Array<Variant>();

        DirAccess dir = DirAccess.Open(path);
        if (dir == null)
        {
            GD.PushError("[DataLoader] Directory not found: " + path);
            return result;
        }

        dir.ListDirBegin();
        string file = dir.GetNext();

        while (!string.IsNullOrEmpty(file))
        {
            if (file.EndsWith(".tres"))
            {
                Resource resource = ResourceLoader.Load(path + file);
                if (resource != null)
                {
                    result.Add(resource);
                }
            }

            file = dir.GetNext();
        }

        dir.ListDirEnd();
        return result;
    }

    private void _append_deep_copies(Godot.Collections.Array<Variant> source, Godot.Collections.Array<Variant> target)
    {
        for (int index = 0; index < source.Count; index++)
        {
            target.Add(this._duplicate_resource(source[index]));
        }
    }

    private Variant _duplicate_resource(Variant resource)
    {
        Resource original = resource.As<Resource>();
        if (original == null)
        {
            return resource;
        }

        return original.Duplicate(true);
    }
}