public class FirstAid : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.status?.Heal(10);
    }
}
