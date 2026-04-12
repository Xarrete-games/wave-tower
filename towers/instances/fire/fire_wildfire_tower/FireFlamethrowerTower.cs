using Godot;

[GlobalClass]
public partial class FireFlamethrowerTower : Tower
{
    private Timer flame_thrower_duration_timer;
    private Node fire_flamethrower_projectile;

    public override void _Ready()
    {
        base._Ready();

        this.flame_thrower_duration_timer = GetNode<Timer>("FlameThrowerDurationTimer");
        this.fire_flamethrower_projectile = GetNode("FireFlamethrowerProjectile");

        this.on_target_change += this._on_new_target_change;
        this.flame_thrower_duration_timer.Timeout += this._on_flame_thrower_duration_timer_timeout;
    }

    public override void _ExitTree()
    {
        this.on_target_change -= this._on_new_target_change;
        if (this.flame_thrower_duration_timer != null)
        {
            this.flame_thrower_duration_timer.Timeout -= this._on_flame_thrower_duration_timer_timeout;
        }

        base._ExitTree();
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target))
        {
            return;
        }

        this.fire_flamethrower_projectile.Call("fire");
        GodotObject attack = this._get_attack();
        this.fire_flamethrower_projectile.Call("set_target", this._current_target, attack);
        this.flame_thrower_duration_timer.Start();
    }

    private void _on_new_target_change(Node2D enemy)
    {
        bool isThrowing = this.fire_flamethrower_projectile.Call("is_throwing").AsBool();
        if (!isThrowing)
        {
            return;
        }

        if (!GodotObject.IsInstanceValid(enemy))
        {
            this.fire_flamethrower_projectile.Call("stop");
            return;
        }

        GodotObject attack = this._get_attack();
        this.fire_flamethrower_projectile.Call("set_target", enemy, attack);
    }

    private void _on_flame_thrower_duration_timer_timeout()
    {
        this.fire_flamethrower_projectile.Call("stop");
    }
}
