public class MagicRing : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        Economy economy = runContext.Economy;
        int current = economy.AvailableFreeTowers;
        economy.AvailableFreeTowers = current + 1;
    }
}
