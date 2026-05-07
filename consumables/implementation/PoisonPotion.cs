public class PoisonPotion : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        runContext.Status.ApplyDamage(10);
    }
}
