public class PoisonPotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.status?.ApplyDamage(10);
    }
}
