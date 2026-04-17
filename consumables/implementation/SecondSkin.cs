public class SecondSkin : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.status?.add_amor(10);
    }
}
