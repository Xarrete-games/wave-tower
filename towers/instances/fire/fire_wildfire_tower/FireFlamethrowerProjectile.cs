using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FireFlamethrowerProjectile : Node2D
{
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    private const float DAMAGE_TICK_INTERVAL = 0.5f;

    private Node2D _target;
    private GodotObject _attack;
    private readonly Array<Node2D> _tagers_in_area = new();

    private CpuParticles2D _fire_particles;
    private Timer _damage_timer;
    private Area2D _area_2d;
    private Node2D _flamethrower;

    public override void _Ready()
    {
        this._fire_particles = GetNode<CpuParticles2D>("%FireParticles");
        this._damage_timer = GetNode<Timer>("%DamageTimer");
        this._area_2d = GetNode<Area2D>("%Area2D");
        this._flamethrower = GetNode<Node2D>("%Flamethrower");

        this._damage_timer.WaitTime = DAMAGE_TICK_INTERVAL;
        this.stop();
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(this._target))
        {
            return;
        }

        Vector2 targetPosition = this._target.Get("target_position").AsVector2();
        Vector2 dir = targetPosition - GlobalPosition;
        this._flamethrower.Rotation = dir.Angle();
    }

    public void fire()
    {
        this._damage_timer.Start();
        this._fire_particles.Emitting = true;
        this._area_2d.Monitoring = true;
    }

    public void set_target(Variant enemy, Variant attack)
    {
        this._target = enemy.AsGodotObject() as Node2D;
        this._attack = attack.AsGodotObject();
    }

    public void stop()
    {
        this._fire_particles.Emitting = false;
        this._area_2d.Monitoring = false;
        this._damage_timer.Stop();
        this._tagers_in_area.Clear();
    }

    public bool is_throwing()
    {
        return this._fire_particles.Emitting;
    }

    private void _on_area_2d_body_exited(Node2D body)
    {
        this._tagers_in_area.Remove(body);
    }

    private void _on_area_2d_body_entered(Node2D body)
    {
        this._tagers_in_area.Add(body);
        body.TreeExited += () => this._tagers_in_area.Remove(body);
    }

    private void _on_damage_timer_timeout()
    {
        foreach (Node2D target in this._tagers_in_area)
        {
            if (!GodotObject.IsInstanceValid(target))
            {
                continue;
            }

            GodotObject source = this._attack?.Get("source").AsGodotObject();
            Variant debuff = EnemyDebuffScript.Call("create_burn", source);
            target.Call("apply_damage", this._attack);
            target.Call("apply_debuff", debuff);
        }
    }
}
