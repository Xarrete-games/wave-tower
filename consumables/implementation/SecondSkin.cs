using Godot;

[GlobalClass]
public partial class SecondSkin : ConsumableUsable
{
    public override void use()
    {
        GodotObject status = this.GetSingleton("RunContext")?.Get("status").AsGodotObject();
        status?.Call("add_amor", 10);
    }
}
