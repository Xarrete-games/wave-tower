public class LootContext
{
    public int base_gold
    {
        get => this.BaseGold;
        set => this.BaseGold = value;
    }

    public int extra_gold
    {
        get => this.ExtraGold;
        set => this.ExtraGold = value;
    }

    public int gold_mult
    {
        get => this.GoldMultiplier;
        set => this.GoldMultiplier = value;
    }

    public int chance_drop_consumable
    {
        get => this.ChanceDropConsumable;
        set => this.ChanceDropConsumable = value;
    }

    public int BaseGold { get; set; }
    public int ExtraGold { get; set; }
    public int GoldMultiplier { get; set; } = 1;
    public int ChanceDropConsumable { get; set; }

    public LootContext(int baseGold = 0, int chanceDropConsumable = 50)
    {
        this.BaseGold = baseGold;
        this.ChanceDropConsumable = chanceDropConsumable;
    }

    public int GetTotalGold()
    {
        return (this.BaseGold + this.ExtraGold) * this.GoldMultiplier;
    }

    public int get_total_gold() => this.GetTotalGold();
}
