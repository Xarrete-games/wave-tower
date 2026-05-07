public class SecondSkin : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = RunContext.Instance;
        runContext.Status.AddArmor(10);
    }
}
