using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BurnArea : Area2D
{
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    private readonly Array<Node2D> _enemies = new();

    [Export] public float duration = 0.3f;

    public GodotObject source;

    private Timer _duration_timer;
    private GpuParticles2D _explosion_particles;
    private CpuParticles2D _cpu_explosion;

    public override void _Ready()
    {
        this._duration_timer = GetNode<Timer>("DurationTimer");
        this._explosion_particles = GetNode<GpuParticles2D>("ExplosionParticles");
        this._cpu_explosion = GetNode<CpuParticles2D>("CPUExplosion");

        this._duration_timer.WaitTime = this.duration;
        Monitoring = false;
    }

    public void setup(Variant p_source)
    {
        this.source = p_source.AsGodotObject();

        this._duration_timer.Start();
        Monitoring = true;
        this._explosion_particles.Restart();
        this._explosion_particles.Emitting = true;
        this._cpu_explosion.Restart();
        this._cpu_explosion.Emitting = true;
    }

    private void _on_body_entered(Node2D body)
    {
        this._enemies.Add(body);
        body.TreeExited += () => this._enemies.Remove(body);

        if (!GodotObject.IsInstanceValid(body))
        {
            return;
        }

        Variant debuff = EnemyDebuffScript.Call("create_burn", this.source);
        body.Call("apply_debuff", debuff);
    }

    private void _on_duration_timer_timeout()
    {
        QueueFree();
    }
}
