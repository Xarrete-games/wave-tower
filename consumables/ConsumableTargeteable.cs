public abstract class ConsumableTargeteable : Consumable
{
    public enum TargetType
    {
        BLOCKED_TILE,
        TOWER,
    }

    public object target { get; private set; }

    public override bool requires_target()
    {
        return true;
    }

    public void use(object p_target)
    {
        this.target = p_target;
        this.action(this.target);
        this.emit_used();
    }

    public Tower get_target_tower()
    {
        return this.target as Tower;
    }

    public abstract void action(object p_target);
}
