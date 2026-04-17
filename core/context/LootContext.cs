public class LootContext
{
    public int base_gold
    {
        get => BaseGold;
        set => BaseGold = value;
    }

    public int extra_gold
    {
        get => ExtraGold;
        set => ExtraGold = value;
    }

    public int gold_mult
    {
        get => GoldMultiplier;
        set => GoldMultiplier = value;
    }

    public int chance_drop_consumable
    {
        get => ChanceDropConsumable;
        set => ChanceDropConsumable = value;
    }

    public int BaseGold { get; set; }
    public int ExtraGold { get; set; }
    public int GoldMultiplier { get; set; } = 1;
    public int ChanceDropConsumable { get; set; }

    public LootContext(int baseGold = 0, int chanceDropConsumable = 50)
    {
        BaseGold = baseGold;
        ChanceDropConsumable = chanceDropConsumable;
    }

    public int GetTotalGold()
    {
        return (BaseGold + ExtraGold) * GoldMultiplier;
    }

    public int get_total_gold() => GetTotalGold();
}
