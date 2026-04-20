public class PoisonPotion : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.ApplyDamage(10);
    }
}
