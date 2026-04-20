using Godot;

[GlobalClass]
public partial class FireFlamethrowerTower : Tower
{
    private Timer flame_thrower_duration_timer;
    private FireFlamethrowerProjectile fire_flamethrower_projectile;

    public override void _Ready()
    {
        base._Ready();

        flame_thrower_duration_timer = GetNode<Timer>("FlameThrowerDurationTimer");
        fire_flamethrower_projectile = GetNode<FireFlamethrowerProjectile>("FireFlamethrowerProjectile");

        TargetChanged += OnNewTargetChange;
        flame_thrower_duration_timer.Timeout += OnFlameThrowerDurationTimerTimeout;
    }

    public override void _ExitTree()
    {
        TargetChanged -= OnNewTargetChange;
        if (flame_thrower_duration_timer != null)
        {
            flame_thrower_duration_timer.Timeout -= OnFlameThrowerDurationTimerTimeout;
        }

        base._ExitTree();
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget))
        {
            return;
        }

        fire_flamethrower_projectile.Fire();
        Attack attack = GetAttack();
        fire_flamethrower_projectile.SetTarget(_currentTarget, attack);
        flame_thrower_duration_timer.Start();
    }

    private void OnNewTargetChange(Node2D enemy)
    {
        bool isThrowing = fire_flamethrower_projectile.IsThrowing();
        if (!isThrowing)
        {
            return;
        }

        if (!GodotObject.IsInstanceValid(enemy))
        {
            fire_flamethrower_projectile.Stop();
            return;
        }

        Attack attack = GetAttack();
        fire_flamethrower_projectile.SetTarget(enemy, attack);
    }

    private void OnFlameThrowerDurationTimerTimeout()
    {
        fire_flamethrower_projectile.Stop();
    }
}
