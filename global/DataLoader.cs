using Godot;
using System.Collections.Generic;

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

    private readonly List<RelicData> _relics = new();
    private readonly List<EventData> _events = new();
    private readonly List<ConsumableData> _consumables = new();
    private readonly List<EnemyDebuffData> _enemyDebuffs = new();
    private readonly List<BuffData> _towerBuffsData = new();
    private readonly List<MapPieceData> _initialMapPieces = new();
    private readonly List<MapPieceData> _mapPieces = new();
    private readonly List<TowerDataWithInstance> _towerData = new();
    private EnemyDataLoader _enemyData;

    public override void _Ready()
    {
        Instance = this;
        _enemyData = new EnemyDataLoader();

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

    public RelicData GetRelicById(string relicId)
    {
        for (int index = 0; index < _relics.Count; index++)
        {
            RelicData relic = _relics[index];
            if (relic != null && relic.Id == relicId)
            {
                return DuplicateResource(relic);
            }
        }

        return null;
    }

    public List<RelicData> GetAllRelics()
    {
        return DuplicateResources(_relics);
    }

    public List<RelicData> GetRandomRelics(int amount, int? rarity = null, bool includeCursed = false, bool includeOnlyForEvents = false)
    {
        List<RelicData> candidates = GetNotUsedRelics(rarity);
        var filtered = new List<RelicData>();

        for (int index = 0; index < candidates.Count; index++)
        {
            RelicData relicData = candidates[index];
            if (relicData == null)
            {
                continue;
            }

            bool isCursed = relicData.IsCursed;
            bool onlyForEvents = relicData.OnlyForEvents;
            if (!includeCursed && isCursed)
            {
                continue;
            }

            if (!includeOnlyForEvents && onlyForEvents)
            {
                continue;
            }

            filtered.Add(relicData);
        }

        ShuffleInPlace(filtered);

        var result = new List<RelicData>();
        for (int index = 0; index < filtered.Count && result.Count < amount; index++)
        {
            result.Add(DuplicateResource(filtered[index]));
        }

        return result;
    }

    public List<RelicData> GetNotUsedRelics(int? rarity = null, bool? isCursed = null)
    {
        var filtered = new List<RelicData>();
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        RelicsManager relicsManager = runContext?.relics_manager;

        for (int index = 0; index < _relics.Count; index++)
        {
            RelicData relicData = _relics[index];
            if (relicData == null)
            {
                continue;
            }

            if (rarity.HasValue)
            {
                if (relicData.Rarity != rarity.Value)
                {
                    continue;
                }
            }

            if (isCursed.HasValue)
            {
                bool relicIsCursed = relicData.IsCursed;
                bool filterCursed = isCursed.Value;
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
                alreadyOwned = relicsManager.HasRelic(relicData.Id);
            }

            if (!alreadyOwned)
            {
                filtered.Add(DuplicateResource(relicData));
            }
        }

        return filtered;
    }

    public List<RelicData> GetRandomAvailableRelics(int amount, int? rarity = null, bool includeCursed = false, bool includeOnlyForEvents = false)
    {
        List<RelicData> candidates = GetNotUsedRelics(rarity);
        var filtered = new List<RelicData>();

        for (int index = 0; index < candidates.Count; index++)
        {
            RelicData relicData = candidates[index];
            if (relicData == null)
            {
                continue;
            }

            bool isCursed = relicData.IsCursed;
            bool onlyForEvents = relicData.OnlyForEvents;
            if (!includeCursed && isCursed)
            {
                continue;
            }

            if (!includeOnlyForEvents && onlyForEvents)
            {
                continue;
            }

            filtered.Add(relicData);
        }

        ShuffleInPlace(filtered);

        var result = new List<RelicData>();
        for (int index = 0; index < filtered.Count && result.Count < amount; index++)
        {
            result.Add(DuplicateResource(filtered[index]));
        }

        return result;
    }

    public List<EventData> GetAllEvents()
    {
        return DuplicateResources(_events);
    }

    public ConsumableData GetConsumableById(string consumableId)
    {
        for (int index = 0; index < _consumables.Count; index++)
        {
            ConsumableData consumable = _consumables[index];
            if (consumable != null && consumable.Id == consumableId)
            {
                return DuplicateResource(consumable);
            }
        }

        return null;
    }

    public List<ConsumableData> GetAllConsumables()
    {
        return DuplicateResources(_consumables);
    }

    public List<ConsumableData> GetAllConsumablesOfType(int consumableType)
    {
        var filtered = new List<ConsumableData>();
        for (int index = 0; index < _consumables.Count; index++)
        {
            ConsumableData data = _consumables[index];
            if (data != null && data.ConsumableType == consumableType)
            {
                filtered.Add(DuplicateResource(data));
            }
        }

        return filtered;
    }

    public EnemyDebuffData GetDebuffData(int debuffType)
    {
        for (int index = 0; index < _enemyDebuffs.Count; index++)
        {
            EnemyDebuffData debuffData = _enemyDebuffs[index];
            if (debuffData != null && debuffData.DebuffType == debuffType)
            {
                return DuplicateResource(debuffData);
            }
        }

        return null;
    }

    public BuffData GetTowerBuffDataById(string buffId)
    {
        for (int index = 0; index < _towerBuffsData.Count; index++)
        {
            BuffData buffData = _towerBuffsData[index];
            if (buffData != null && buffData.Id == buffId)
            {
                return DuplicateResource(buffData);
            }
        }

        return null;
    }

    public List<MapPieceData> GetAllInitialMapPieces()
    {
        return DuplicateResources(_initialMapPieces);
    }

    public List<MapPieceData> GetAllMapPieces()
    {
        return DuplicateResources(_mapPieces);
    }

    public List<EnemyData> GetAllEnemies()
    {
        if (_enemyData == null)
        {
            return new List<EnemyData>();
        }

        return _enemyData.GetAllEnemies();
    }

    public List<EnemyData> GetEnemiesByType(int type)
    {
        if (_enemyData == null)
        {
            return new List<EnemyData>();
        }

        return _enemyData.GetEnemiesByType(type);
    }

    public List<EnemyData> GetSpawnableEnemies()
    {
        if (_enemyData == null)
        {
            return new List<EnemyData>();
        }

        return _enemyData.GetSpawnableEnemies();
    }

    public List<TowerDataWithInstance> GetAllTowerData()
    {
        return DuplicateResources(_towerData);
    }

    private void LoadRelics()
    {
        var loadedArray = LoadResourcesFromDir(RELICS_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            RelicData data = loadedArray[index] as RelicData;
            if (data != null)
            {
                _relics.Add(data);
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
            EventData data = loadedArray[index] as EventData;
            if (data != null)
            {
                _events.Add(data);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded event data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadConsumables()
    {
        var loadedArray = LoadResourcesFromDir(CONSUMABLES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            ConsumableData data = loadedArray[index] as ConsumableData;
            if (data != null)
            {
                _consumables.Add(data);
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
            EnemyDebuffData data = loadedArray[index] as EnemyDebuffData;
            if (data != null)
            {
                _enemyDebuffs.Add(data);
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
            BuffData data = loadedArray[index] as BuffData;
            if (data != null)
            {
                _towerBuffsData.Add(data);
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
            MapPieceData data = loadedArray[index] as MapPieceData;
            if (data != null)
            {
                _mapPieces.Add(data);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded map piece data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadInitialMapPieces()
    {
        var loadedArray = LoadResourcesFromDir(INITIAL_MAP_PIECES_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            MapPieceData data = loadedArray[index] as MapPieceData;
            if (data != null)
            {
                _initialMapPieces.Add(data);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded initial map piece data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private void LoadTowerData()
    {
        var loadedArray = LoadResourcesFromDir(TOWER_DATA_PATH);
        for (int index = 0; index < loadedArray.Count; index++)
        {
            TowerDataWithInstance data = loadedArray[index] as TowerDataWithInstance;
            if (data != null)
            {
                _towerData.Add(data);
            }
            else
            {
                GD.PushError($"[DataLoader] Loaded tower data has invalid type: {loadedArray[index]}");
            }
        }
    }

    private List<Resource> LoadResourcesFromDir(string path)
    {
        var result = new List<Resource>();

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

    private static List<T> DuplicateResources<T>(List<T> source) where T : Resource
    {
        var result = new List<T>(source.Count);
        for (int index = 0; index < source.Count; index++)
        {
            result.Add(DuplicateResource(source[index]));
        }

        return result;
    }

    private static T DuplicateResource<T>(T resource) where T : Resource
    {
        if (resource == null)
        {
            return null;
        }

        return resource.Duplicate(true) as T;
    }

    private static void ShuffleInPlace<T>(List<T> items)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = (int)(GD.Randi() % (uint)(i + 1));
            T tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }
    }
}

