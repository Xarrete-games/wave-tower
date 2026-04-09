using Godot;
using System.Collections.Generic;

public abstract partial class RelicRuntimeAdapter : RefCounted
{
    [Signal]
    public delegate void changedEventHandler(Variant relic);

    public Variant data { get; set; }

    protected abstract RelicModel Model { get; }

    public bool disabled
    {
        get => this.Model.Disabled;
        set
        {
            this.Model.Disabled = value;
            this.EmitSignal(SignalName.changed, this);
        }
    }

    public int counter
    {
        get => this.Model.Counter;
        set
        {
            this.Model.Counter = value;
            this.EmitSignal(SignalName.changed, this);
        }
    }

    public string id => this.Model.Id;

    public virtual void on_tower_placed(Variant towerInstance)
    {
        this.Model.OnTowerPlaced(new TowerModel());
    }

    public virtual void on_wave_init()
    {
        this.Model.OnWaveInit();
    }

    public virtual void on_wave_finished()
    {
        this.Model.OnWaveFinished();
    }
    public virtual void on_before_damage(Variant context) { }
    public virtual void on_before_attack(Variant context) { }
    public virtual void on_enemy_die(Variant enemy, Variant attack) { }
    public virtual void on_debuff_applied(Variant context, Variant target) { }
    public virtual void on_relic_added(Variant relicAdded) { }
    public virtual void on_consumable_used(Variant consumable) { }
    public virtual void on_before_get_loot(Variant context)
    {
        GodotObject lootContext = context.AsGodotObject();
        if (lootContext == null)
        {
            return;
        }

        int baseGold = (int)lootContext.Get("base_gold");
        int chanceDropConsumable = (int)lootContext.Get("chance_drop_consumable");
        int extraGold = (int)lootContext.Get("extra_gold");
        int goldMultiplier = (int)lootContext.Get("gold_mult");

        var modelContext = new LootContext(baseGold, chanceDropConsumable)
        {
            ExtraGold = extraGold,
            GoldMultiplier = goldMultiplier,
        };

        this.Model.OnBeforeGetLoot(modelContext);

        lootContext.Set("extra_gold", modelContext.ExtraGold);
        lootContext.Set("gold_mult", modelContext.GoldMultiplier);
        lootContext.Set("chance_drop_consumable", modelContext.ChanceDropConsumable);
    }

    public virtual void on_before_relic_reward(Variant context)
    {
        GodotObject rewardsContext = context.AsGodotObject();
        if (rewardsContext == null)
        {
            return;
        }

        int numberOfRelics = (int)rewardsContext.Get("number_of_relics");
        var modelContext = new RelicsRewardsContext(numberOfRelics);

        this.Model.OnBeforeRelicReward(modelContext);
        rewardsContext.Set("number_of_relics", modelContext.NumberOfRelics);
    }
    public virtual void on_before_die(Variant status) { }

    public virtual void on_obtain()
    {
        this.Model.OnObtain();
    }

    public virtual void on_remove()
    {
        this.Model.OnRemove();
    }

    public virtual void on_get_price(Variant context)
    {
        GodotObject priceContext = context.AsGodotObject();
        if (priceContext == null)
        {
            return;
        }

        int priceType = (int)priceContext.Get("price_type");
        int basePrice = (int)priceContext.Get("base_price");
        float discount = (float)priceContext.Get("discount");

        var modelContext = new PriceContext((PriceContext.PriceType)priceType, basePrice)
        {
            Discount = discount,
        };

        this.Model.OnGetPrice(modelContext);
        priceContext.Set("discount", modelContext.Discount);
    }

    public virtual void on_get_targeting_modes(Variant targetingModes)
    {
        if (targetingModes.VariantType != Variant.Type.Array)
        {
            return;
        }

        var modesArray = targetingModes.AsGodotArray();
        var modelModes = new List<TowerTargetingMode>();

        for (int index = 0; index < modesArray.Count; index++)
        {
            int modeValue = (int)modesArray[index];
            modelModes.Add((TowerTargetingMode)modeValue);
        }

        this.Model.OnGetTargetingModes(modelModes);

        modesArray.Clear();
        for (int index = 0; index < modelModes.Count; index++)
        {
            modesArray.Add((int)modelModes[index]);
        }
    }
}
