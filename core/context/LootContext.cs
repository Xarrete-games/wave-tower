public class LootContext
{
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
}
