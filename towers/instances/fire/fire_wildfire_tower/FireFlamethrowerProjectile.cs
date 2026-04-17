using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FireFlamethrowerProjectile : Node2D
{
    private const float DAMAGE_TICK_INTERVAL = 0.5f;

    private Node2D _target;
    private Attack _attack;
    private readonly Array<Node2D> _tagers_in_area = new();

    private CpuParticles2D _fire_particles;
    private Timer _damage_timer;
    private Area2D _area_2d;
    private Node2D _flamethrower;

    public override void _Ready()
    {
        _fire_particles = GetNode<CpuParticles2D>("%FireParticles");
        _damage_timer = GetNode<Timer>("%DamageTimer");
        _area_2d = GetNode<Area2D>("%Area2D");
        _flamethrower = GetNode<Node2D>("%Flamethrower");

        _damage_timer.WaitTime = DAMAGE_TICK_INTERVAL;
        stop();
    }

    public override void _Process(double delta)
    {
        Enemy targetEnemy = _target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy))
        {
            return;
        }

        Vector2 targetPosition = targetEnemy.target_position;
        Vector2 dir = targetPosition - GlobalPosition;
        _flamethrower.Rotation = dir.Angle();
    }

    public void fire()
    {
        _damage_timer.Start();
        _fire_particles.Emitting = true;
        _area_2d.Monitoring = true;
    }

    public void set_target(Node2D enemy, Attack attack)
    {
        _target = enemy;
        _attack = attack;
    }

    public void stop()
    {
        _fire_particles.Emitting = false;
        _area_2d.Monitoring = false;
        _damage_timer.Stop();
        _tagers_in_area.Clear();
    }

    public bool is_throwing()
    {
        return _fire_particles.Emitting;
    }

    private void OnArea2dBodyExited(Node2D body)
    {
        _tagers_in_area.Remove(body);
    }

    private void OnArea2dBodyEntered(Node2D body)
    {
        _tagers_in_area.Add(body);
        body.TreeExited += () => _tagers_in_area.Remove(body);
    }

    private void OnDamageTimerTimeout()
    {
        foreach (Node2D target in _tagers_in_area)
        {
            if (!GodotObject.IsInstanceValid(target))
            {
                continue;
            }

            Source source = _attack?.source;
            EnemyDebuff debuff = source != null ? EnemyDebuff.create_burn(source) : null;
            Enemy enemyModel = target as Enemy;
            enemyModel?.apply_damage(_attack);
            if (debuff != null)
            {
                enemyModel?.apply_debuff(debuff);
            }
        }
    }
}
