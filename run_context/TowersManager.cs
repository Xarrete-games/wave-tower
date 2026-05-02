using Godot;
using System.Collections.Generic;
using System;

public class TowersManager
{
    public event Action<int, int> TowerCountChanged;
    public event Action<TowerDataWithInstance, int> TowerCardAmountChanged;
    public event Action<Tower> TowerPlaced;
    public event Action<Tower> TowerHovered;
    public event Action<Tower> TowerUnhovered;
    public event Action<Tower> TowerSelected;
    public event Action<Tower> TowerRemoved;

    private static readonly string[] INITIAL_TOWERS_IDS = { "fire_tower", "frost_tower", "lightning_tower" };
    private const float COMMON_WEIGHT_START = 0.75f;
    private const float RARE_WEIGHT_START = 0.20f;
    private const float EPIC_WEIGHT_START = 0.05f;
    private const float COMMON_WEIGHT_END = 0.34f;
    private const float RARE_WEIGHT_END = 0.33f;
    private const float EPIC_WEIGHT_END = 0.33f;

    public Dictionary<string, int> LastTowerIds { get; } = new();
    public List<string> TowersIds { get; } = new();
    public List<Tower> Towers { get; } = new();
    public List<TowerDataWithInstance> AllTowerData { get; private set; } = new();
    public Dictionary<string, int> TowerCardsAmount { get; } = new();

    private RunProgress _progress;
    private readonly Dictionary<ulong, TowerModel> _runtimeTowerModels = new();
    private readonly Dictionary<ulong, HashSet<string>> _appliedRuntimeBuffSources = new();

    public TowersManager()
    {
        ClickEvents.TowerRemovePressed += OnTowerRemoved;
        ClickEvents.AddTowerCard += OnTowerCardAdded;

        AllTowerData = DataLoaderAccess.GetAllTowerDataTyped();
    }

    public void DisposeEvents()
    {
        ClickEvents.TowerRemovePressed -= OnTowerRemoved;
        ClickEvents.AddTowerCard -= OnTowerCardAdded;
    }

    public void Setup(RunProgress progress)
    {
        _progress = progress;
        LastTowerIds.Clear();
        TowersIds.Clear();
        Towers.Clear();
        TowerCardsAmount.Clear();
        _runtimeTowerModels.Clear();
        _appliedRuntimeBuffSources.Clear();
        InitInitialTowersData();
    }

    public List<TowerDataWithInstance> GetRandomTowers(int amount)
    {
        var availableTowers = new List<TowerDataWithInstance>(AllTowerData);
        var selectedTowers = new List<TowerDataWithInstance>();
        int picks = Mathf.Min(amount, availableTowers.Count);

        for (int i = 0; i < picks; i++)
        {
            float totalWeight = 0f;
            var weights = new List<float>(availableTowers.Count);

            for (int index = 0; index < availableTowers.Count; index++)
            {
                TowerDataWithInstance towerData = availableTowers[index];
                if (towerData?.Data == null)
                {
                    weights.Add(0f);
                    continue;
                }

                int rarity = (int)towerData.Data.Rarity;
                float weight = GetTowerWeightForWave(rarity);
                weights.Add(weight);
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
            {
                int randomIndex = (int)(GD.Randi() % (uint)availableTowers.Count);
                TowerDataWithInstance fallbackTower = availableTowers[randomIndex];
                if (fallbackTower?.Data != null)
                {
                    selectedTowers.Add(fallbackTower);
                }
                availableTowers.RemoveAt(randomIndex);
                continue;
            }

            float roll = GD.Randf() * totalWeight;
            float cumulativeWeight = 0f;
            int selectedIndex = 0;

            for (int index = 0; index < availableTowers.Count; index++)
            {
                cumulativeWeight += weights[index];
                if (roll <= cumulativeWeight)
                {
                    selectedIndex = index;
                    break;
                }
            }

            TowerDataWithInstance selectedTower = availableTowers[selectedIndex];
            if (selectedTower?.Data != null)
            {
                selectedTowers.Add(selectedTower);
            }
            availableTowers.RemoveAt(selectedIndex);
        }

        return selectedTowers;
    }

    public TowerDataWithInstance GetTowerConfigurationById(string id)
    {
        for (int index = 0; index < AllTowerData.Count; index++)
        {
            TowerDataWithInstance typedConfiguration = AllTowerData[index];
            if (typedConfiguration?.Data == null)
            {
                continue;
            }

            if (typedConfiguration.Data.Id == id)
            {
                return typedConfiguration;
            }
        }

        return null;
    }

    public void AddTowerPlaced(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        Towers.Add(tower);

        int towerType = (int)tower.TowerType;
        UpdateTowerCount(towerType);

        TowerData towerData = tower.Data as TowerData;
        if (towerData == null)
        {
            GD.PushError("[TowersManager] Placed tower has no TowerData.");
            return;
        }

        string towerDataId = towerData.Id;
        int currentAmount = TowerCardsAmount.ContainsKey(towerDataId) ? TowerCardsAmount[towerDataId] : 0;
        TowerCardsAmount[towerDataId] = currentAmount - 1;

        TowerDataWithInstance towerConfiguration = GetTowerConfigurationById(towerDataId);
        TowerCardAmountChanged?.Invoke(towerConfiguration, TowerCardsAmount[towerDataId]);

        tower.Id = GenerateTowerId(tower);

        TowerModel towerModel = BuildTowerModel(tower);
        if (towerModel != null)
        {
            ulong instanceId = tower.GetInstanceId();
            _runtimeTowerModels[instanceId] = towerModel;
            RunContextRuntime.TowersManager.AddTowerPlaced(towerModel, instanceId);

            SyncRuntimeStatusFromLegacy();
            Hooks.OnTowerPlaced(Hooks.GetListenersFromRuntime(), towerModel);
            SyncLegacyStatusFromRuntime();
            ApplyRuntimeBuffsToLegacyTower(instanceId, tower, towerModel);
        }

        TowerPlaced?.Invoke(tower);
        GetAudioManager()?.PlayPlaceTower();
    }

    public void OnTowerRemoved(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        Towers.Remove(tower);

        int towerType = (int)tower.TowerType;
        UpdateTowerCount(towerType);

        TowersIds.Remove(tower.Id);

        ulong instanceId = tower.GetInstanceId();
        if (_runtimeTowerModels.ContainsKey(instanceId))
        {
            _runtimeTowerModels.Remove(instanceId);
        }

        if (_appliedRuntimeBuffSources.ContainsKey(instanceId))
        {
            _appliedRuntimeBuffSources.Remove(instanceId);
        }

        RunContextRuntime.TowersManager.RemoveTowerByInstanceId(instanceId);

        TowerRemoved?.Invoke(tower);
        tower.QueueFree();
    }

    public int GetTowerCount(int towerType)
    {
        int count = 0;
        for (int index = 0; index < Towers.Count; index++)
        {
            Tower tower = Towers[index];
            if (tower != null && (int)tower.TowerType == towerType)
            {
                count++;
            }
        }

        return count;
    }

    public List<Tower> GetPlacedTowers()
    {
        var placedTowers = new List<Tower>();
        for (int index = 0; index < Towers.Count; index++)
        {
            Tower tower = Towers[index];
            if (tower != null)
            {
                placedTowers.Add(tower);
            }
        }

        return placedTowers;
    }

    public void ResetTowers()
    {
        UpdateTowerCount(0);
        UpdateTowerCount(1);
        UpdateTowerCount(2);
    }

    public void SelectTower(Tower tower)
    {
        TowerSelected?.Invoke(tower);
        ClickEvents.TowerSelected?.Invoke(tower);
    }

    public void OnTowerCardAdded(TowerDataWithInstance towerData)
    {
        if (towerData == null || towerData.Data == null)
        {
            GD.PushError($"[TowersManager] Invalid tower configuration while adding card: {towerData}");
            return;
        }

        string id = towerData.Data.Id;
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        int amount = TowerCardsAmount.ContainsKey(id) ? TowerCardsAmount[id] : 0;
        TowerCardsAmount[id] = amount + 1;
        TowerCardAmountChanged?.Invoke(towerData, TowerCardsAmount[id]);
    }

    public void EmitTowerHovered(Tower tower)
    {
        TowerHovered?.Invoke(tower);
        ClickEvents.TowerHovered?.Invoke(tower);
    }

    public void EmitTowerUnhovered(Tower tower)
    {
        TowerUnhovered?.Invoke(tower);
        ClickEvents.TowerUnhovered?.Invoke(tower);
    }

    public void SyncRuntimeBuffsForTower(ulong instanceId)
    {
        if (!_runtimeTowerModels.TryGetValue(instanceId, out TowerModel towerModel))
        {
            return;
        }

        Tower tower = GodotObject.InstanceFromId(instanceId) as Tower;
        if (tower == null)
        {
            return;
        }

        ApplyRuntimeBuffsToLegacyTower(instanceId, tower, towerModel);
    }

    public void SyncRuntimeBuffsForAllTowers()
    {
        foreach (KeyValuePair<ulong, TowerModel> entry in _runtimeTowerModels)
        {
            ulong instanceId = entry.Key;
            Tower tower = GodotObject.InstanceFromId(instanceId) as Tower;
            if (tower == null)
            {
                continue;
            }

            ApplyRuntimeBuffsToLegacyTower(instanceId, tower, entry.Value);
        }
    }

    private float GetTowerWeightForWave(int rarity)
    {
        float progressRatio = GetWaveProgressRatio();
        float commonWeight = Mathf.Lerp(COMMON_WEIGHT_START, COMMON_WEIGHT_END, progressRatio);
        float rareWeight = Mathf.Lerp(RARE_WEIGHT_START, RARE_WEIGHT_END, progressRatio);
        float epicWeight = Mathf.Lerp(EPIC_WEIGHT_START, EPIC_WEIGHT_END, progressRatio);

        return rarity switch
        {
            0 => commonWeight,
            1 => rareWeight,
            2 => epicWeight,
            _ => commonWeight,
        };
    }

    private float GetWaveProgressRatio()
    {
        if (_progress == null)
        {
            return 0f;
        }

        int totalWaves = _progress.TotalWaves;
        if (totalWaves <= 0)
        {
            return 0f;
        }

        int currentWave = _progress.CurrentWave;
        return Mathf.Clamp((float)currentWave / totalWaves, 0f, 1f);
    }

    private void UpdateTowerCount(int towerType)
    {
        TowerCountChanged?.Invoke(towerType, GetTowerCount(towerType));
    }

    private void InitInitialTowersData()
    {
        for (int index = 0; index < INITIAL_TOWERS_IDS.Length; index++)
        {
            TowerDataWithInstance towerConfiguration = GetTowerConfigurationById(INITIAL_TOWERS_IDS[index]);
            if (towerConfiguration != null)
            {
                OnTowerCardAdded(towerConfiguration);
            }
        }
    }

    private string GenerateTowerId(Tower tower)
    {
        if (tower == null)
        {
            return string.Empty;
        }

        string baseId = tower.TypeId;

        if (!LastTowerIds.ContainsKey(baseId))
        {
            LastTowerIds[baseId] = 0;
        }

        int count = LastTowerIds[baseId] + 1;
        string newId = $"{baseId}_{count}";
        while (TowersIds.Contains(newId))
        {
            count += 1;
            newId = $"{baseId}_{count}";
        }

        TowersIds.Add(newId);
        LastTowerIds[baseId] = count;
        return newId;
    }

    private TowerModel BuildTowerModel(Tower tower)
    {
        if (tower == null)
        {
            return null;
        }

        int rawType = (int)tower.TowerType;
        TowerModel.TowerType towerType = rawType switch
        {
            0 => TowerModel.TowerType.Fire,
            1 => TowerModel.TowerType.Lightning,
            2 => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        var model = new TowerModel
        {
            Id = tower.Id,
            TypeId = tower.TypeId,
            Type = towerType,
        };

        return model;
    }

    private Node GetSingleton(string name)
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
    }

    private AudioManager GetAudioManager()
    {
        return GetSingleton("AudioManager") as AudioManager;
    }

    private void SyncRuntimeStatusFromLegacy()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        Status status = runContext?.Status;
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.MaxHealth, status.Health, status.Armor);
    }

    private void SyncLegacyStatusFromRuntime()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        Status status = runContext?.Status;
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.MaxHealth != runtime.MaxHealth)
        {
            status.MaxHealth = runtime.MaxHealth;
        }

        if (status.Armor != runtime.Armor)
        {
            status.Armor = runtime.Armor;
        }

        if (status.Health != runtime.Health)
        {
            status.Health = runtime.Health;
        }
    }

    private void ApplyRuntimeBuffsToLegacyTower(ulong instanceId, Tower tower, TowerModel towerModel)
    {
        if (tower == null || towerModel == null)
        {
            return;
        }

        if (!_appliedRuntimeBuffSources.TryGetValue(instanceId, out HashSet<string> appliedSources))
        {
            appliedSources = new HashSet<string>();
            _appliedRuntimeBuffSources[instanceId] = appliedSources;
        }

        var buffs = towerModel.GetBuffs();
        HashSet<string> activeSources = new HashSet<string>();
        for (int index = 0; index < buffs.Count; index++)
        {
            TowerBuffModel buff = buffs[index];
            if (buff == null || string.IsNullOrEmpty(buff.SourceId))
            {
                continue;
            }

            activeSources.Add(buff.SourceId);
            if (appliedSources.Contains(buff.SourceId))
            {
                continue;
            }

            Source source = new Source(Source.SourceType.RELIC, buff.SourceId);
            TowerBuff legacyBuff = TowerBuffFactory.CreateFromId(buff.Id, source, buff.Value);
            if (legacyBuff == null)
            {
                continue;
            }

            tower.AddBuff(legacyBuff);
            appliedSources.Add(buff.SourceId);
        }

        if (appliedSources.Count == 0)
        {
            return;
        }

        var staleSources = new List<string>();
        foreach (string sourceId in appliedSources)
        {
            if (!activeSources.Contains(sourceId))
            {
                staleSources.Add(sourceId);
            }
        }

        for (int index = 0; index < staleSources.Count; index++)
        {
            string staleSourceId = staleSources[index];
            tower.RemoveBuff(staleSourceId);
            appliedSources.Remove(staleSourceId);
        }
    }
}



