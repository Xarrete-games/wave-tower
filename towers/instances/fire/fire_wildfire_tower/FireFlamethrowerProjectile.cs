using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FireFlamethrowerProjectile : Node2D
{
    private const float DAMAGE_TICK_INTERVAL = 0.5f;

    private Node2D _target;
    private Attack _attack;
    private readonly Array<Node2D> _targetsInArea = new();

    private CpuParticles2D _fire_particles;
    private Timer _damage_timer;
    private Area2D _area2D;
    private Node2D _flamethrower;

    public override void _Ready()
    {
        _fire_particles = GetNode<CpuParticles2D>("%FireParticles");
        _damage_timer = GetNode<Timer>("%DamageTimer");
        _area2D = GetNode<Area2D>("%Area2D");
        _flamethrower = GetNode<Node2D>("%Flamethrower");

        _damage_timer.WaitTime = DAMAGE_TICK_INTERVAL;
        Stop();
    }

    public override void _Process(double delta)
    {
        Enemy targetEnemy = _target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy))
        {
            return;
        }

        Vector2 targetPosition = targetEnemy.TargetPosition;
        Vector2 dir = targetPosition - GlobalPosition;
        _flamethrower.Rotation = dir.Angle();
    }

    public void Fire()
    {
        _damage_timer.Start();
        _fire_particles.Emitting = true;
        _area2D.Monitoring = true;
    }

    public void SetTarget(Node2D enemy, Attack attack)
    {
        _target = enemy;
        _attack = attack;
    }

    public void Stop()
    {
        _fire_particles.Emitting = false;
        _area2D.Monitoring = false;
        _damage_timer.Stop();
        _targetsInArea.Clear();
    }

    public bool IsThrowing()
    {
        return _fire_particles.Emitting;
    }

    private void OnArea2dBodyExited(Node2D body)
    {
        _targetsInArea.Remove(body);
    }

    private void OnArea2dBodyEntered(Node2D body)
    {
        _targetsInArea.Add(body);
        body.TreeExited += () => _targetsInArea.Remove(body);
    }

    private void OnDamageTimerTimeout()
    {
        foreach (Node2D target in _targetsInArea)
        {
            if (!GodotObject.IsInstanceValid(target))
            {
                continue;
            }

            Source source = _attack?.source;
            EnemyDebuff debuff = source != null ? EnemyDebuff.create_burn(source) : null;
            Enemy enemy = target as Enemy;
            enemy?.ApplyDamage(_attack);
            if (debuff != null)
            {
                enemy?.ApplyDebuff(debuff);
            }
        }
    }
}
