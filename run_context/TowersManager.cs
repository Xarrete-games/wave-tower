using Godot;
using System.Collections.Generic;
using System;

public class TowersManager
{
    public event Action<int, int> tower_count_change;
    public event Action<TowerDataWithInstance, int> tower_card_amount_change;
    public event Action<Tower> tower_placed;
    public event Action<Tower> tower_hovered;
    public event Action<Tower> tower_unhovered;
    public event Action<Tower> tower_selected;
    public event Action<Tower> tower_removed;

    private static readonly string[] INITIAL_TOWERS_IDS = { "fire_tower", "frost_tower", "lightning_tower" };
    private const float COMMON_WEIGHT_START = 0.75f;
    private const float RARE_WEIGHT_START = 0.20f;
    private const float EPIC_WEIGHT_START = 0.05f;
    private const float COMMON_WEIGHT_END = 0.34f;
    private const float RARE_WEIGHT_END = 0.33f;
    private const float EPIC_WEIGHT_END = 0.33f;

    public Godot.Collections.Dictionary<string, int> last_tower_ids { get; } = new();
    public Godot.Collections.Array<string> towers_ids { get; } = new();
    public List<Tower> towers { get; } = new();
    public List<TowerDataWithInstance> all_tower_data { get; private set; } = new();
    public Godot.Collections.Dictionary<string, int> tower_cards_amount { get; } = new();

    private RunProgress _progress;
    private readonly Dictionary<ulong, TowerModel> _runtimeTowerModels = new();
    private readonly Dictionary<ulong, HashSet<string>> _appliedRuntimeBuffSources = new();

    public TowersManager()
    {
        ClickEvents.TowerRemovePressed += this.OnTowerRemoved;
        ClickEvents.AddTowerCard += this._on_tower_card_added;

        this.all_tower_data = DataLoaderAccess.GetAllTowerDataTyped();
    }

    public void dispose_events()
    {
        ClickEvents.TowerRemovePressed -= this.OnTowerRemoved;
        ClickEvents.AddTowerCard -= this._on_tower_card_added;
    }

    public void setup(RunProgress progress_p)
    {
        this._progress = progress_p;
        this.last_tower_ids.Clear();
        this.towers_ids.Clear();
        this.towers.Clear();
        this.tower_cards_amount.Clear();
        this._runtimeTowerModels.Clear();
        this._appliedRuntimeBuffSources.Clear();
        this._init_inital_towers_data();
    }

    public List<TowerDataWithInstance> get_random_towers(int amount)
    {
        var availableTowers = new List<TowerDataWithInstance>(this.all_tower_data);
        var selectedTowers = new List<TowerDataWithInstance>();
        int picks = Mathf.Min(amount, availableTowers.Count);

        for (int i = 0; i < picks; i++)
        {
            float totalWeight = 0f;
            var weights = new List<float>(availableTowers.Count);

            for (int index = 0; index < availableTowers.Count; index++)
            {
                TowerDataWithInstance towerData = availableTowers[index];
                if (towerData?.data == null)
                {
                    weights.Add(0f);
                    continue;
                }

                int rarity = (int)towerData.data.rarity;
                float weight = this._get_tower_weight_for_wave(rarity);
                weights.Add(weight);
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
            {
                int randomIndex = (int)(GD.Randi() % (uint)availableTowers.Count);
                TowerDataWithInstance fallbackTower = availableTowers[randomIndex];
                if (fallbackTower?.data != null)
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
            if (selectedTower?.data != null)
            {
                selectedTowers.Add(selectedTower);
            }
            availableTowers.RemoveAt(selectedIndex);
        }

        return selectedTowers;
    }

    public TowerDataWithInstance get_tower_configuration_by_id(string id)
    {
        for (int index = 0; index < this.all_tower_data.Count; index++)
        {
            TowerDataWithInstance typedConfiguration = this.all_tower_data[index];
            if (typedConfiguration?.data == null)
            {
                continue;
            }

            if (typedConfiguration.data.id == id)
            {
                return typedConfiguration;
            }
        }

        return null;
    }

    public void add_tower_placed(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        this.towers.Add(tower);

        int towerType = (int)tower.type;
        this._update_tower_count(towerType);

        TowerData towerData = tower.data as TowerData;
        if (towerData == null)
        {
            GD.PushError("[TowersManager] Placed tower has no TowerData.");
            return;
        }

        string towerDataId = towerData.id;
        int currentAmount = this.tower_cards_amount.ContainsKey(towerDataId) ? this.tower_cards_amount[towerDataId] : 0;
        this.tower_cards_amount[towerDataId] = currentAmount - 1;

        TowerDataWithInstance towerConfiguration = this.get_tower_configuration_by_id(towerDataId);
        this.tower_card_amount_change?.Invoke(towerConfiguration, this.tower_cards_amount[towerDataId]);

        tower.id = this._generate_tower_id(tower);

        TowerModel towerModel = this.BuildTowerModel(tower);
        if (towerModel != null)
        {
            ulong instanceId = tower.GetInstanceId();
            this._runtimeTowerModels[instanceId] = towerModel;
            RunContextRuntime.TowersManager.AddTowerPlaced(towerModel, instanceId);

            this.SyncRuntimeStatusFromLegacy();
            Hooks.OnTowerPlaced(Hooks.GetListenersFromRuntime(), towerModel);
            this.SyncLegacyStatusFromRuntime();
            this.ApplyRuntimeBuffsToLegacyTower(instanceId, tower, towerModel);
        }

        this.tower_placed?.Invoke(tower);
        this.GetAudioManager()?.play_place_tower();
    }

    public void OnTowerRemoved(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        this.towers.Remove(tower);

        int towerType = (int)tower.type;
        this._update_tower_count(towerType);

        this.towers_ids.Remove(tower.id);

        ulong instanceId = tower.GetInstanceId();
        if (this._runtimeTowerModels.ContainsKey(instanceId))
        {
            this._runtimeTowerModels.Remove(instanceId);
        }

        if (this._appliedRuntimeBuffSources.ContainsKey(instanceId))
        {
            this._appliedRuntimeBuffSources.Remove(instanceId);
        }

        RunContextRuntime.TowersManager.RemoveTowerByInstanceId(instanceId);

        this.tower_removed?.Invoke(tower);
        tower.QueueFree();
    }

    public int get_tower_count(int tower_type)
    {
        int count = 0;
        for (int index = 0; index < this.towers.Count; index++)
        {
            Tower tower = this.towers[index];
            if (tower != null && (int)tower.type == tower_type)
            {
                count++;
            }
        }

        return count;
    }

    public List<Tower> get_placed_towers()
    {
        var placedTowers = new List<Tower>();
        for (int index = 0; index < this.towers.Count; index++)
        {
            Tower tower = this.towers[index];
            if (tower != null)
            {
                placedTowers.Add(tower);
            }
        }

        return placedTowers;
    }

    public void reset_towers()
    {
        this._update_tower_count(0);
        this._update_tower_count(1);
        this._update_tower_count(2);
    }

    public void select_tower(Tower tower)
    {
        this.tower_selected?.Invoke(tower);
        ClickEvents.TowerSelected?.Invoke(tower);
    }

    public void _on_tower_card_added(TowerDataWithInstance tower_data)
    {
        if (tower_data == null || tower_data.data == null)
        {
            GD.PushError($"[TowersManager] Invalid tower configuration while adding card: {tower_data}");
            return;
        }

        string id = tower_data.data.id;
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        int amount = this.tower_cards_amount.ContainsKey(id) ? this.tower_cards_amount[id] : 0;
        this.tower_cards_amount[id] = amount + 1;
        this.tower_card_amount_change?.Invoke(tower_data, this.tower_cards_amount[id]);
    }

    public void emit_tower_hovered(Tower tower)
    {
        this.tower_hovered?.Invoke(tower);
        ClickEvents.TowerHovered?.Invoke(tower);
    }

    public void emit_tower_unhovered(Tower tower)
    {
        this.tower_unhovered?.Invoke(tower);
        ClickEvents.TowerUnhovered?.Invoke(tower);
    }

    public void sync_runtime_buffs_for_tower(ulong instanceId)
    {
        if (!this._runtimeTowerModels.TryGetValue(instanceId, out TowerModel towerModel))
        {
            return;
        }

        Tower tower = GodotObject.InstanceFromId(instanceId) as Tower;
        if (tower == null)
        {
            return;
        }

        this.ApplyRuntimeBuffsToLegacyTower(instanceId, tower, towerModel);
    }

    private float _get_tower_weight_for_wave(int rarity)
    {
        float progressRatio = this._get_wave_progress_ratio();
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

    private float _get_wave_progress_ratio()
    {
        if (this._progress == null)
        {
            return 0f;
        }

        int totalWaves = this._progress.total_waves;
        if (totalWaves <= 0)
        {
            return 0f;
        }

        int currentWave = this._progress.current_wave;
        return Mathf.Clamp((float)currentWave / totalWaves, 0f, 1f);
    }

    private void _update_tower_count(int tower_type)
    {
        this.tower_count_change?.Invoke(tower_type, this.get_tower_count(tower_type));
    }

    private void _init_inital_towers_data()
    {
        for (int index = 0; index < INITIAL_TOWERS_IDS.Length; index++)
        {
            TowerDataWithInstance towerConfiguration = this.get_tower_configuration_by_id(INITIAL_TOWERS_IDS[index]);
            if (towerConfiguration != null)
            {
                this._on_tower_card_added(towerConfiguration);
            }
        }
    }

    private string _generate_tower_id(Tower tower)
    {
        if (tower == null)
        {
            return string.Empty;
        }

        string baseId = tower.type_id;

        if (!this.last_tower_ids.ContainsKey(baseId))
        {
            this.last_tower_ids[baseId] = 0;
        }

        int count = this.last_tower_ids[baseId] + 1;
        string newId = $"{baseId}_{count}";
        while (this.towers_ids.Contains(newId))
        {
            count += 1;
            newId = $"{baseId}_{count}";
        }

        this.towers_ids.Add(newId);
        this.last_tower_ids[baseId] = count;
        return newId;
    }

    private TowerModel BuildTowerModel(Tower tower)
    {
        if (tower == null)
        {
            return null;
        }

        int rawType = (int)tower.type;
        TowerModel.TowerType towerType = rawType switch
        {
            0 => TowerModel.TowerType.Fire,
            1 => TowerModel.TowerType.Lightning,
            2 => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        var model = new TowerModel
        {
            Id = tower.id,
            TypeId = tower.type_id,
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
        return this.GetSingleton("AudioManager") as AudioManager;
    }

    private void SyncRuntimeStatusFromLegacy()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        Status status = runContext?.status;
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.max_health, status.health, status.armor);
    }

    private void SyncLegacyStatusFromRuntime()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        Status status = runContext?.status;
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.max_health != runtime.MaxHealth)
        {
            status.max_health = runtime.MaxHealth;
        }

        if (status.armor != runtime.Armor)
        {
            status.armor = runtime.Armor;
        }

        if (status.health != runtime.Health)
        {
            status.health = runtime.Health;
        }
    }

    private void ApplyRuntimeBuffsToLegacyTower(ulong instanceId, Tower tower, TowerModel towerModel)
    {
        if (tower == null || towerModel == null)
        {
            return;
        }

        if (!this._appliedRuntimeBuffSources.TryGetValue(instanceId, out HashSet<string> appliedSources))
        {
            appliedSources = new HashSet<string>();
            this._appliedRuntimeBuffSources[instanceId] = appliedSources;
        }

        var buffs = towerModel.GetBuffs();
        for (int index = 0; index < buffs.Count; index++)
        {
            TowerBuffModel buff = buffs[index];
            if (buff == null || string.IsNullOrEmpty(buff.SourceId) || appliedSources.Contains(buff.SourceId))
            {
                continue;
            }

            Source source = new Source(Source.SourceType.RELIC, buff.SourceId);
            TowerBuff legacyBuff = TowerBuffFactory.create_from_id(buff.Id, source, buff.Value);
            if (legacyBuff == null)
            {
                continue;
            }

            tower.add_buff(legacyBuff);
            appliedSources.Add(buff.SourceId);
        }
    }
}

