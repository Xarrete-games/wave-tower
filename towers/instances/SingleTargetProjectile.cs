using Godot;
[GlobalClass] public partial class SingleTargetProjectile : Node2D {
    private const float SPEED = 1200.0f;
    private const float HIT_RADIUS = 12.0f;
    private Node2D _enemy;
    private Attack _attack;
    private EnemyDebuff _debuff;
    private int _debuffStacks = 1;
    public override void _Process(double delta) {
        Enemy enemy = _enemy as Enemy;
        if (!GodotObject.IsInstanceValid(enemy)) {
            QueueFree();
            return;
        }
        Vector2 targetPosition = enemy.TargetPosition;
        Vector2 direction = targetPosition - GlobalPosition;
        float distance = direction.Length();
        if (distance <= HIT_RADIUS) {
            enemy?.ApplyDamage(_attack);
            if (_debuff != null) {
                enemy?.ApplyDebuff(_debuff, _debuffStacks);
            }
            QueueFree();
            return;
        }
        GlobalPosition += direction.Normalized() * SPEED * (float)delta;
        LookAt(targetPosition);
    }
    public void SetTarget(Node2D enemy, Attack attack) {
        SetTarget(enemy, attack, null, 1);
    }
    public void SetTarget(Node2D enemy, Attack attack, EnemyDebuff debuff) {
        SetTarget(enemy, attack, debuff, 1);
    }
    public void SetTarget(Node2D enemy, Attack attack, EnemyDebuff debuff, int debuffStacks) {
        _enemy = enemy;
        _attack = attack;
        _debuff = debuff;
        _debuffStacks = debuffStacks;
    }
}

