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

        on_target_change += OnNewTargetChange;
        flame_thrower_duration_timer.Timeout += OnFlameThrowerDurationTimerTimeout;
    }

    public override void _ExitTree()
    {
        on_target_change -= OnNewTargetChange;
        if (flame_thrower_duration_timer != null)
        {
            flame_thrower_duration_timer.Timeout -= OnFlameThrowerDurationTimerTimeout;
        }

        base._ExitTree();
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target))
        {
            return;
        }

        fire_flamethrower_projectile.fire();
        Attack attack = _get_attack();
        fire_flamethrower_projectile.set_target(_current_target, attack);
        flame_thrower_duration_timer.Start();
    }

    private void OnNewTargetChange(Node2D enemy)
    {
        bool isThrowing = fire_flamethrower_projectile.is_throwing();
        if (!isThrowing)
        {
            return;
        }

        if (!GodotObject.IsInstanceValid(enemy))
        {
            fire_flamethrower_projectile.stop();
            return;
        }

        Attack attack = _get_attack();
        fire_flamethrower_projectile.set_target(enemy, attack);
    }

    private void OnFlameThrowerDurationTimerTimeout()
    {
        fire_flamethrower_projectile.stop();
    }
}
