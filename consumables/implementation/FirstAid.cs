using Godot;

[GlobalClass]
public partial class FirstAid : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        runContext?.status?.heal(10);
    }
}
