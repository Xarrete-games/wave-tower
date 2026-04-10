using Godot;

[GlobalClass]
public abstract partial class ConsumableTargeteable : Consumable
{
    public enum TargetType
    {
        BLOCKED_TILE,
        TOWER,
    }

    public Variant target { get; private set; }

    public override bool requires_target()
    {
        return true;
    }

    public void use(Variant p_target)
    {
        this.target = p_target;
        this.action(this.target);
        this.EmitSignal(SignalName.used, this);
    }

    public abstract void action(Variant p_target);
}
