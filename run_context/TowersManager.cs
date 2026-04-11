using Godot;

[GlobalClass]
public partial class TowersManager : RefCounted
{
    [Signal]
    public delegate void tower_count_changeEventHandler(int tower_type, int amount);

    [Signal]
    public delegate void tower_card_amount_changeEventHandler(Variant tower_configuration, int amount);

    [Signal]
    public delegate void tower_placedEventHandler(Variant tower);

    [Signal]
    public delegate void tower_hoveredEventHandler(Variant tower);

    [Signal]
    public delegate void tower_unhoveredEventHandler(Variant tower);

    [Signal]
    public delegate void tower_selectedEventHandler(Variant tower);

    [Signal]
    public delegate void tower_removedEventHandler(Variant tower);

    private static readonly Script _hooksScript = GD.Load<Script>("res://core/hooks.gd");

    private static readonly string[] INITIAL_TOWERS_IDS = { "fire_tower", "frost_tower", "lightning_tower" };
    private const float COMMON_WEIGHT_START = 0.75f;
    private const float RARE_WEIGHT_START = 0.20f;
    private const float EPIC_WEIGHT_START = 0.05f;
    private const float COMMON_WEIGHT_END = 0.34f;
    private const float RARE_WEIGHT_END = 0.33f;
    private const float EPIC_WEIGHT_END = 0.33f;

    public Godot.Collections.Dictionary<string, int> last_tower_ids { get; } = new();
    public Godot.Collections.Array<string> towers_ids { get; } = new();
    public Godot.Collections.Array<Variant> towers { get; } = new();
    public Godot.Collections.Array<Variant> all_tower_data { get; private set; } = new();
    public Godot.Collections.Dictionary<string, int> tower_cards_amount { get; } = new();

    private Variant _progress;

    public TowersManager()
    {
        ClickEventsBus.TowerRemovePressed += this.OnTowerRemoved;
        ClickEventsBus.AddTowerCard += this._on_tower_card_added;

        this.all_tower_data = DataLoaderAccess.GetAllTowerData();
    }

    public void dispose_events()
    {
        ClickEventsBus.TowerRemovePressed -= this.OnTowerRemoved;
        ClickEventsBus.AddTowerCard -= this._on_tower_card_added;
    }

    public void setup(Variant progress_p)
    {
        this._progress = progress_p;
        this.last_tower_ids.Clear();
        this.towers_ids.Clear();
        this.towers.Clear();
        this.tower_cards_amount.Clear();
        this._init_inital_towers_data();
    }

    public Godot.Collections.Array<Variant> get_random_towers(int amount)
    {
        var availableTowers = this.all_tower_data.Duplicate();
        var selectedTowers = new Godot.Collections.Array<Variant>();
        int picks = Mathf.Min(amount, availableTowers.Count);

        for (int i = 0; i < picks; i++)
        {
            float totalWeight = 0f;
            var weights = new Godot.Collections.Array<float>();

            for (int index = 0; index < availableTowers.Count; index++)
            {
                Variant towerData = availableTowers[index];
                if (!this._has_configuration_data(towerData))
                {
                    weights.Add(0f);
                    continue;
                }

                GodotObject cfg = towerData.AsGodotObject();
                GodotObject data = cfg.Get("data").AsGodotObject();
                int rarity = (int)data.Get("rarity");
                float weight = this._get_tower_weight_for_wave(rarity);
                weights.Add(weight);
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
            {
                availableTowers.Shuffle();
                selectedTowers.Add(availableTowers[availableTowers.Count - 1]);
                availableTowers.RemoveAt(availableTowers.Count - 1);
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

            selectedTowers.Add(availableTowers[selectedIndex]);
            availableTowers.RemoveAt(selectedIndex);
        }

        return selectedTowers;
    }

    public Variant get_tower_configuration_by_id(string id)
    {
        for (int index = 0; index < this.all_tower_data.Count; index++)
        {
            Variant towerConfiguration = this.all_tower_data[index];
            if (!this._has_configuration_data(towerConfiguration))
            {
                continue;
            }

            GodotObject cfg = towerConfiguration.AsGodotObject();
            GodotObject data = cfg.Get("data").AsGodotObject();
            if ((string)data.Get("id") == id)
            {
                return towerConfiguration;
            }
        }

        return default;
    }

    public void add_tower_placed(Variant tower)
    {
        _hooksScript.Call("on_tower_placed", tower);
        this.towers.Add(tower);

        GodotObject towerObj = tower.AsGodotObject();
        int towerType = (int)towerObj.Get("type");
        this._update_tower_count(towerType);

        GodotObject towerData = towerObj.Get("data").AsGodotObject();
        string towerDataId = (string)towerData.Get("id");
        int currentAmount = this.tower_cards_amount.ContainsKey(towerDataId) ? this.tower_cards_amount[towerDataId] : 0;
        this.tower_cards_amount[towerDataId] = currentAmount - 1;

        Variant towerConfiguration = this.get_tower_configuration_by_id(towerDataId);
        this.EmitSignal(SignalName.tower_card_amount_change, towerConfiguration, this.tower_cards_amount[towerDataId]);

        towerObj.Set("id", this._generate_tower_id(tower));
        this.EmitSignal(SignalName.tower_placed, tower);
        this.GetSingleton("AudioManager")?.Call("play_place_tower");
    }

    public void OnTowerRemoved(Variant tower)
    {
        this.towers.Remove(tower);

        GodotObject towerObj = tower.AsGodotObject();
        int towerType = (int)towerObj.Get("type");
        this._update_tower_count(towerType);

        this.towers_ids.Remove((string)towerObj.Get("id"));
        this.EmitSignal(SignalName.tower_removed, tower);
        towerObj.Call("queue_free");
    }

    public int get_tower_count(int tower_type)
    {
        int count = 0;
        for (int index = 0; index < this.towers.Count; index++)
        {
            GodotObject towerObj = this.towers[index].AsGodotObject();
            if (towerObj != null && (int)towerObj.Get("type") == tower_type)
            {
                count++;
            }
        }

        return count;
    }

    public Godot.Collections.Array<Variant> get_tower_listeners()
    {
        var listeners = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < this.towers.Count; index++)
        {
            GodotObject towerObj = this.towers[index].AsGodotObject();
            if (towerObj == null)
            {
                continue;
            }

            Variant towerLogic = towerObj.Get("tower_logic");
            if (towerLogic.VariantType != Variant.Type.Nil)
            {
                listeners.Add(towerLogic);
            }
        }

        return listeners;
    }

    public void reset_towers()
    {
        this._update_tower_count(0);
        this._update_tower_count(1);
        this._update_tower_count(2);
    }

    public void select_tower(Variant tower)
    {
        this.EmitSignal(SignalName.tower_selected, tower);
        ClickEventsBus.EmitTowerSelected(tower);
    }

    public void _on_tower_card_added(Variant tower_data)
    {
        if (!this._has_configuration_data(tower_data))
        {
            GD.PushError($"[TowersManager] Invalid tower configuration while adding card: {tower_data}");
            return;
        }

        GodotObject cfg = tower_data.AsGodotObject();
        GodotObject data = cfg.Get("data").AsGodotObject();
        string id = (string)data.Get("id");

        int amount = this.tower_cards_amount.ContainsKey(id) ? this.tower_cards_amount[id] : 0;
        this.tower_cards_amount[id] = amount + 1;
        this.EmitSignal(SignalName.tower_card_amount_change, tower_data, this.tower_cards_amount[id]);
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
        GodotObject progressObj = this._progress.AsGodotObject();
        if (progressObj == null)
        {
            return 0f;
        }

        int totalWaves = (int)progressObj.Get("total_waves");
        if (totalWaves <= 0)
        {
            return 0f;
        }

        int currentWave = (int)progressObj.Get("current_wave");
        return Mathf.Clamp((float)currentWave / totalWaves, 0f, 1f);
    }

    private void _update_tower_count(int tower_type)
    {
        this.EmitSignal(SignalName.tower_count_change, tower_type, this.get_tower_count(tower_type));
    }

    private void _init_inital_towers_data()
    {
        var initialCards = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < INITIAL_TOWERS_IDS.Length; index++)
        {
            Variant towerConfiguration = this.get_tower_configuration_by_id(INITIAL_TOWERS_IDS[index]);
            if (towerConfiguration.VariantType != Variant.Type.Nil)
            {
                initialCards.Add(towerConfiguration);
            }
        }

        for (int index = 0; index < initialCards.Count; index++)
        {
            this._on_tower_card_added(initialCards[index]);
        }
    }

    private bool _has_configuration_data(Variant tower_configuration)
    {
        GodotObject cfg = tower_configuration.AsGodotObject();
        if (cfg == null)
        {
            return false;
        }

        Variant data = cfg.Get("data");
        return data.VariantType != Variant.Type.Nil;
    }

    private string _generate_tower_id(Variant tower)
    {
        GodotObject towerObj = tower.AsGodotObject();
        string baseId = (string)towerObj.Get("type_id");

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

    private Node GetSingleton(string name)
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
    }
}
