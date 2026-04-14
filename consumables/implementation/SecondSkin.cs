using Godot;

public class SecondSkin : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        runContext?.status?.add_amor(10);
    }
}
