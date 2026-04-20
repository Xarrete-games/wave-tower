public class SecondSkin : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.AddArmor(10);
    }
}
