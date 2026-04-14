public class HealingPotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        runContext?.status?.heal(15);
    }
}
