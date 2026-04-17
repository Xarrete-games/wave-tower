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
    public EnemyDataLoader enemy_data;

    public override void _Ready()
    {
        Instance = this;
        enemy_data = new EnemyDataLoader();

        LoadRelics();
        LoadEvents();
        LoadConsumables();
        LoadEnemyDebuffs();
        LoadTowerBuffData();
        LoadMapPieces();
        LoadInitialMapPieces();
        LoadTowerData();
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
        for (int index = 0; index < relics.Count; index++)
        {
            RelicData relic = relics[index].As<RelicData>();
            if (relic != null && relic.id == relic_id)
            {
                return DuplicateResource(relics[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_relics()
    {
        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(relics, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_random_relics(int amount, Variant rarity = default, bool include_cursed = false, bool include_OnlyForEvents = false)
    {
        var candidates = get_not_used_relics(rarity);
        var filtered = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < candidates.Count; index++)
        {
            RelicData relicData = candidates[index].As<RelicData>();
            if (relicData == null)
            {
                continue;
            }

            bool isCursed = relicData.IsCursed;
            bool onlyForEvents = relicData.OnlyForEvents;
            if (!include_cursed && isCursed)
            {
                continue;
            }

            if (!include_OnlyForEvents && onlyForEvents)
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

    public Godot.Collections.Array<Variant> get_not_used_relics(Variant rarity = default, Variant IsCursed = default)
    {
        var filtered = new Godot.Collections.Array<Variant>();
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        RelicsManager relicsManager = runContext?.relics_manager;

        for (int index = 0; index < relics.Count; index++)
        {
            RelicData relicData = relics[index].As<RelicData>();
            if (relicData == null)
            {
                continue;
            }

            if (rarity.VariantType != Variant.Type.Nil)
            {
                if (relicData.rarity != (int)rarity)
                {
                    continue;
                }
            }

            if (IsCursed.VariantType != Variant.Type.Nil)
            {
                bool relicIsCursed = relicData.IsCursed;
                bool filterCursed = (bool)IsCursed;
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
                alreadyOwned = relicsManager.has_relic(relicData.id);
            }

            if (!alreadyOwned)
            {
                filtered.Add(relics[index]);
            }
        }

        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(filtered, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_random_available_relics(int amount, Variant rarity = default, bool include_cursed = false, bool include_OnlyForEvents = false)
    {
        var candidates = get_not_used_relics(rarity);
        var filtered = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < candidates.Count; index++)
        {
            RelicData relicData = candidates[index].As<RelicData>();
            if (relicData == null)
            {
                continue;
            }

            bool isCursed = relicData.IsCursed;
            bool onlyForEvents = relicData.OnlyForEvents;
            if (!include_cursed && isCursed)
            {
                continue;
            }

            if (!include_OnlyForEvents && onlyForEvents)
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
        AppendDeepCopies(events, result);
        return result;
    }

    public Variant get_consumable_by_id(string consumable_id)
    {
        for (int index = 0; index < consumables.Count; index++)
        {
            ConsumableData consumable = consumables[index].As<ConsumableData>();
            if (consumable != null && consumable.id == consumable_id)
            {
                return DuplicateResource(consumables[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_consumables()
    {
        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(consumables, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_consumables_of_type(int ConsumableType)
    {
        var filtered = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < consumables.Count; index++)
        {
            ConsumableData data = consumables[index].As<ConsumableData>();
            if (data != null && data.ConsumableType == ConsumableType)
            {
                filtered.Add(consumables[index]);
            }
        }

        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(filtered, result);
        return result;
    }

    public Variant get_debuff_data(int type)
    {
        for (int index = 0; index < enemy_debuffs.Count; index++)
        {
            EnemyDebuffData debuffData = enemy_debuffs[index].As<EnemyDebuffData>();
            if (debuffData != null && debuffData.DebuffType == type)
            {
                return DuplicateResource(enemy_debuffs[index]);
            }
        }

        return default;
    }

    public Variant get_tower_buff_data_by_id(string buff_id)
    {
        for (int index = 0; index < tower_buffs_data.Count; index++)
        {
            BuffData buffData = tower_buffs_data[index].As<BuffData>();
            if (buffData != null && buffData.id == buff_id)
            {
                return DuplicateResource(tower_buffs_data[index]);
            }
        }

        return default;
    }

    public Godot.Collections.Array<Variant> get_all_initial_map_pieces()
    {
        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(initial_map_pieces, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_map_pieces()
    {
        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(map_pieces, result);
        return result;
    }

    public Godot.Collections.Array<Variant> get_all_enemies()
    {
        if (enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return enemy_data.get_all_enemies();
    }

    public Godot.Collections.Array<Variant> get_enemies_by_type(int type)
    {
        if (enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return enemy_data.get_enemies_by_type(type);
    }

    public Godot.Collections.Array<Variant> get_spawnable_enemies()
    {
        if (enemy_data == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return enemy_data.get_spawnable_enemies();
    }

    public Godot.Collections.Array<Variant> get_all_tower_data()
    {
        var result = new Godot.Collections.Array<Variant>();
        AppendDeepCopies(tower_data, result);
        return result;
    }

    private void LoadRelics()
    {
        var loadedArray = LoadResourcesFromDir(RELICS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && HasAnyProperty(data, "Id", "id") && HasProperty(data, "RuntimeScript"))
            {
                relics.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded relic has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadEvents()
    {
        var loadedArray = LoadResourcesFromDir(EVENTS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            events.Add(loadedArray[index]);
        }
    }

    private void LoadConsumables()
    {
        var loadedArray = LoadResourcesFromDir(CONSUMABLES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && HasAnyProperty(data, "Id", "id") && HasProperty(data, "ConsumableType"))
            {
                consumables.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded consumables data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadEnemyDebuffs()
    {
        var loadedArray = LoadResourcesFromDir(ENEMY_DEBUFFS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && HasAnyProperty(data, "Id", "id") && HasProperty(data, "DebuffType"))
            {
                enemy_debuffs.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded enemy debuff data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadTowerBuffData()
    {
        var loadedArray = LoadResourcesFromDir(TOWER_BUFFS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && HasAnyProperty(data, "Id", "id") && HasProperty(data, "RuntimeScript"))
            {
                tower_buffs_data.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded tower buff data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadMapPieces()
    {
        var loadedArray = LoadResourcesFromDir(MAP_PIECES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            map_pieces.Add(loadedArray[index]);
        }
    }

    private void LoadInitialMapPieces()
    {
        var loadedArray = LoadResourcesFromDir(INITIAL_MAP_PIECES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            initial_map_pieces.Add(loadedArray[index]);
        }
    }

    private void LoadTowerData()
    {
        var loadedArray = LoadResourcesFromDir(TOWER_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            GodotObject data = loadedArray[index].AsGodotObject();
            if (data != null && HasAnyProperty(data, "Data", "data") && HasAnyProperty(data, "Scene", "scene"))
            {
                tower_data.Add(loadedArray[index]);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded tower data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private bool HasProperty(GodotObject target, string propertyName)
    {
        if (target == null)
        {
            return false;
        }

        var properties = target.GetPropertyList();
        for (int index = 0; index < properties.Count; index++)
        {
            var dict = properties[index];
            if (dict.ContainsKey("name") && dict["name"].AsString() == propertyName)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasAnyProperty(GodotObject target, params string[] propertyNames)
    {
        for (int index = 0; index < propertyNames.Length; index++)
        {
            if (HasProperty(target, propertyNames[index]))
            {
                return true;
            }
        }

        return false;
    }

    private Godot.Collections.Array<Variant> LoadResourcesFromDir(string path)
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

    private void AppendDeepCopies(Godot.Collections.Array<Variant> source, Godot.Collections.Array<Variant> target)
    {
        for (int index = 0; index < source.Count; index++)
        {
            target.Add(DuplicateResource(source[index]));
        }
    }

    private Variant DuplicateResource(Variant resource)
    {
        Resource original = resource.As<Resource>();
        if (original == null)
        {
            return resource;
        }

        return original.Duplicate(true);
    }
}

