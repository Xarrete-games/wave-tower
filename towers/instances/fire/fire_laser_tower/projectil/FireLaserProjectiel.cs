using Godot;

[GlobalClass]
public partial class FireLaserProjectiel : Node2D
{
    [Export] public int cast_speed = 7000;
    [Export] public Color color = Colors.Red;

    [Export] public float growth_time = 0.1f;

    private Node2D _target;
    private Attack _attack;
    private EnemyDebuff _debuff;
    private int _amount_debuff = 1;
    private Tween _tween;
    private float _current_laser_length;
    private bool _is_casting;

    private Line2D _line_2d;
    private float _line_width;
    private AudioStreamPlayer2D _red_attack_start;
    private AudioStreamPlayer2D _red_attack_loop;
    private AudioStreamPlayer2D _red_attack_finish;
    private CpuParticles2D _fire_particles;

    private readonly float _amplitude = 10f;
    private readonly float _speed = -16f;
    private readonly float _phase_offset = Mathf.Pi / 3f;
    private Color _color = Colors.Red;

    public override void _Ready()
    {
        this._line_2d = GetNode<Line2D>("Line2D");
        this._line_width = this._line_2d.Width;
        this._red_attack_start = GetNode<AudioStreamPlayer2D>("RedAttack_start");
        this._red_attack_loop = GetNode<AudioStreamPlayer2D>("RedAttack_loop");
        this._red_attack_finish = GetNode<AudioStreamPlayer2D>("RedAttack_finish");
        this._fire_particles = GetNode<CpuParticles2D>("FireParticles");

        this.set_color(this.color);
        this._fire_particles.Emitting = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!this._is_casting || !GodotObject.IsInstanceValid(this._target))
        {
            return;
        }

        Vector2 targetPosition = this._target.Get("target_position").AsVector2();
        LookAt(targetPosition);

        float distanceToTarget = GlobalPosition.DistanceTo(targetPosition);
        this._fire_particles.GlobalPosition = targetPosition;

        this._current_laser_length = Mathf.MoveToward(this._current_laser_length, distanceToTarget, this.cast_speed * (float)delta);

        int iMax = this._line_2d.GetPointCount() - 1;
        if (iMax <= 0)
        {
            return;
        }

        float time = Time.GetTicksMsec() / 1000.0f;
        for (int i = 0; i < this._line_2d.GetPointCount(); i++)
        {
            Vector2 pos = this._line_2d.GetPointPosition(i);
            pos.X = i * this._current_laser_length / iMax;
            pos.Y = Mathf.Sin(time * this._speed + i * this._phase_offset) * this._amplitude * Mathf.Sin(Mathf.Pi * i / iMax);
            this._line_2d.SetPointPosition(i, pos);
        }
    }

    public void stop()
    {
        this._target = null;
        this._set_is_casting(false);
    }

    public void set_target(Node2D target, Attack attack)
    {
        this.set_target(target, attack, null, 1);
    }

    public void set_target(Node2D target, Attack attack, EnemyDebuff debuff)
    {
        this.set_target(target, attack, debuff, 1);
    }

    public void set_target(Node2D target, Attack attack, EnemyDebuff debuff, int amount)
    {
        if (target == this._target)
        {
            return;
        }

        this._target = target;
        this._attack = attack;
        this._debuff = debuff;
        this._amount_debuff = amount;

        if (!this._is_casting)
        {
            this._set_is_casting(true);
        }
    }

    public void hit_target()
    {
        if (!GodotObject.IsInstanceValid(this._target))
        {
            return;
        }

        Enemy enemyModel = this._target as Enemy;
        enemyModel?.apply_damage(this._attack);
        if (this._debuff != null)
        {
            enemyModel?.apply_debuff(this._debuff, this._amount_debuff);
        }
    }

    public void set_color(Color new_color)
    {
        this._color = new_color;
        if (this._line_2d != null)
        {
            this._line_2d.Modulate = new_color;
        }
    }

    private void _set_is_casting(bool new_value)
    {
        if (this._is_casting == new_value)
        {
            return;
        }

        this._is_casting = new_value;
        if (!this._is_casting)
        {
            this._dissapear();
            return;
        }

        this._current_laser_length = 0.0f;
        this._appear();
    }

    private void _dissapear()
    {
        if (this._red_attack_loop.IsInsideTree())
        {
            this._red_attack_loop.Stop();
        }

        if (this._red_attack_finish.IsInsideTree())
        {
            this._red_attack_finish.Play();
        }

        if (this._line_2d == null)
        {
            return;
        }

        if (this._tween != null && this._tween.IsRunning())
        {
            this._tween.Kill();
        }

        this._fire_particles.Emitting = false;
        this._tween = CreateTween();
        this._tween.TweenProperty(this._line_2d, "width", 0.0f, this.growth_time * 2.0f).FromCurrent();
        this._tween.Finished += this._on_disappear_tween_finished;
    }

    private void _on_disappear_tween_finished()
    {
        this._line_2d?.Hide();
        this._current_laser_length = 0.0f;
    }

    private async void _appear()
    {
        this._red_attack_start.Play();
        if (this._line_2d == null)
        {
            return;
        }

        this._line_2d.Visible = true;
        if (this._tween != null && this._tween.IsRunning())
        {
            this._tween.Kill();
        }

        this._fire_particles.Emitting = true;
        this._tween = CreateTween();
        this._tween.TweenProperty(this._line_2d, "width", this._line_width, this.growth_time * 2.0f).From(0.0f);

        await ToSignal(this._red_attack_start, AudioStreamPlayer2D.SignalName.Finished);
        if (this._is_casting)
        {
            this._red_attack_loop.Play();
        }
    }
}
