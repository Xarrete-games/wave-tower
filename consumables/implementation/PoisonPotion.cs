public class PoisonPotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.ApplyDamage(10);
    }
}
