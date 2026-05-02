using Godot;

public abstract class ConsumableTargeteable : Consumable
{
    public enum TargetType
    {
        BLOCKED_TILE,
        TOWER,
    }

    public Node Target { get; private set; }

    public override bool RequiresTarget()
    {
        return true;
    }

    public void Use(Node selectedTarget)
    {
        Target = selectedTarget;
        Action(Target);
        EmitUsed();
    }

    public Tower GetTargetTower()
    {
        return (Tower)Target;
    }

    public abstract void Action(Node target);
}

