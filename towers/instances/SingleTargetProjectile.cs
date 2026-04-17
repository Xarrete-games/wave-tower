using Godot;
[GlobalClass] public partial class SingleTargetProjectile : Node2D {
    private const float SPEED = 1200.0f;
    private const float HIT_RADIUS = 12.0f;
    private Node2D _enemy;
    private Attack _attack;
    private EnemyDebuff _debuff;
    private int _debuffStacks = 1;
    public override void _Process(double delta) {
        Enemy enemyModel = _enemy as Enemy;
        if (!GodotObject.IsInstanceValid(enemyModel)) {
            QueueFree();
            return;
        }
        Vector2 targetPosition = enemyModel.target_position;
        Vector2 direction = targetPosition - GlobalPosition;
        float distance = direction.Length();
        if (distance <= HIT_RADIUS) {
            enemyModel?.apply_damage(_attack);
            if (_debuff != null) {
                enemyModel?.apply_debuff(_debuff, _debuffStacks);
            }
            QueueFree();
            return;
        }
        GlobalPosition += direction.Normalized() * SPEED * (float)delta;
        LookAt(targetPosition);
    }
    public void set_target(Node2D enemy, Attack attack) {
        set_target(enemy, attack, null, 1);
    }
    public void set_target(Node2D enemy, Attack attack, EnemyDebuff debuff) {
        set_target(enemy, attack, debuff, 1);
    }
    public void set_target(Node2D enemy, Attack attack, EnemyDebuff debuff, int debuff_stacks) {
        _enemy = enemy;
        _attack = attack;
        _debuff = debuff;
        _debuffStacks = debuff_stacks;
    }
}

