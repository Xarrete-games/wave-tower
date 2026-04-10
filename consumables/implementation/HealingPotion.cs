using Godot;

[GlobalClass]
public partial class HealingPotion : ConsumableUsable
{
    public override void use()
    {
        GodotObject status = this.GetSingleton("RunContext")?.Get("status").AsGodotObject();
        status?.Call("heal", 15);
    }
}
