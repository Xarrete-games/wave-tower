public class HealingPotion : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        runContext?.Status?.Heal(15);
    }
}
