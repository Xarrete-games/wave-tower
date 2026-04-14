using Godot;

[GlobalClass]
public partial class SingleTargetProjectile : Node2D
{
    private const float SPEED = 1200.0f;
    private const float HIT_RADIUS = 12.0f;

    private Node2D _enemy;
    private Attack _attack;
    private EnemyDebuff _debuff;
    private int _debuffStacks = 1;

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(this._enemy))
        {
            QueueFree();
            return;
        }

        Vector2 targetPosition = this._enemy.Get("target_position").AsVector2();
        Vector2 direction = targetPosition - GlobalPosition;
        float distance = direction.Length();

        if (distance <= HIT_RADIUS)
        {
            Enemy enemyModel = this._enemy as Enemy;
            enemyModel?.apply_damage(this._attack);
            if (this._debuff != null)
            {
                enemyModel?.apply_debuff(this._debuff, this._debuffStacks);
            }

            QueueFree();
            return;
        }

        GlobalPosition += direction.Normalized() * SPEED * (float)delta;
        LookAt(targetPosition);
    }

    public void set_target(Node2D p_enemy, Attack p_attack)
    {
        this.set_target(p_enemy, p_attack, null, 1);
    }

    public void set_target(Node2D p_enemy, Attack p_attack, EnemyDebuff p_debuff)
    {
        this.set_target(p_enemy, p_attack, p_debuff, 1);
    }

    public void set_target(Node2D p_enemy, Attack p_attack, EnemyDebuff p_debuff, int p_debuff_stacks)
    {
        this._enemy = p_enemy;
        this._attack = p_attack;
        this._debuff = p_debuff;
        this._debuffStacks = p_debuff_stacks;
    }
}
