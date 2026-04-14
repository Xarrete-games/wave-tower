using Godot;

public class PoisonPotion : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        runContext?.status?.apply_damage(10);
    }
}
