public class MagicRing : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        Economy economy = runContext?.Economy;
        if (economy == null)
        {
            return;
        }

        int current = economy.AvailableFreeTowers;
        economy.AvailableFreeTowers = current + 1;
    }
}
