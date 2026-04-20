public class FirstAid : ConsumableUsable
{
    public override void Use()
    {
        RunContext runContext = GetSingleton("RunContext") as RunContext;
        runContext?.Status?.Heal(10);
    }
}
