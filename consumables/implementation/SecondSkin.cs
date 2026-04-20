public class SecondSkin : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.AddArmor(10);
    }
}
