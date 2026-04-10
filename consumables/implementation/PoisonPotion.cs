using Godot;

[GlobalClass]
public partial class PoisonPotion : ConsumableUsable
{
    public override void use()
    {
        GodotObject status = this.GetSingleton("RunContext")?.Get("status").AsGodotObject();
        status?.Call("apply_damage", 10);
    }
}
