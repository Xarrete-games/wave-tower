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

    public TowerNode GetTargetTower()
    {
        return Target as TowerNode;
    }

    public abstract void Action(Node target);
}
