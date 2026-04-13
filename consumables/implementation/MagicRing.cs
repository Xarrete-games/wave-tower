using Godot;

[GlobalClass]
public partial class MagicRing : ConsumableUsable
{
    public override void use()
    {
        RunContext runContext = this.GetSingleton("RunContext") as RunContext;
        Economy economy = runContext?.economy;
        if (economy == null)
        {
            return;
        }

        int current = economy.available_free_towers;
        economy.available_free_towers = current + 1;
    }
}
