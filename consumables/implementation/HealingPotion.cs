public class HealingPotion : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.Heal(15);
    }
}
