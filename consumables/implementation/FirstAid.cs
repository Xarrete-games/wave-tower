public class FirstAid : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        runContext.Status.Heal(10);
    }
}
