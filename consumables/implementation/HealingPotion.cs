public class HealingPotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.Heal(15);
    }
}
