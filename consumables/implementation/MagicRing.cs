using Godot;

[GlobalClass]
public partial class MagicRing : ConsumableUsable
{
    public override void use()
    {
        GodotObject economy = this.GetSingleton("RunContext")?.Get("economy").AsGodotObject();
        if (economy == null)
        {
            return;
        }

        int current = (int)economy.Get("available_free_towers");
        economy.Set("available_free_towers", current + 1);
    }
}
