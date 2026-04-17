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
        _line_2d = GetNode<Line2D>("Line2D");
        _line_width = _line_2d.Width;
        _red_attack_start = GetNode<AudioStreamPlayer2D>("RedAttack_start");
        _red_attack_loop = GetNode<AudioStreamPlayer2D>("RedAttack_loop");
        _red_attack_finish = GetNode<AudioStreamPlayer2D>("RedAttack_finish");
        _fire_particles = GetNode<CpuParticles2D>("FireParticles");

        set_color(color);
        _fire_particles.Emitting = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        Enemy targetEnemy = _target as Enemy;
        if (!_is_casting || !GodotObject.IsInstanceValid(targetEnemy))
        {
            return;
        }

        Vector2 targetPosition = targetEnemy.target_position;
        LookAt(targetPosition);

        float distanceToTarget = GlobalPosition.DistanceTo(targetPosition);
        _fire_particles.GlobalPosition = targetPosition;

        _current_laser_length = Mathf.MoveToward(_current_laser_length, distanceToTarget, cast_speed * (float)delta);

        int iMax = _line_2d.GetPointCount() - 1;
        if (iMax <= 0)
        {
            return;
        }

        float time = Time.GetTicksMsec() / 1000.0f;
        for (int i = 0; i < _line_2d.GetPointCount(); i++)
        {
            Vector2 pos = _line_2d.GetPointPosition(i);
            pos.X = i * _current_laser_length / iMax;
            pos.Y = Mathf.Sin(time * _speed + i * _phase_offset) * _amplitude * Mathf.Sin(Mathf.Pi * i / iMax);
            _line_2d.SetPointPosition(i, pos);
        }
    }

    public void stop()
    {
        _target = null;
        SetIsCasting(false);
    }

    public void set_target(Node2D target, Attack attack)
    {
        set_target(target, attack, null, 1);
    }

    public void set_target(Node2D target, Attack attack, EnemyDebuff debuff)
    {
        set_target(target, attack, debuff, 1);
    }

    public void set_target(Node2D target, Attack attack, EnemyDebuff debuff, int amount)
    {
        if (target == _target)
        {
            return;
        }

        _target = target;
        _attack = attack;
        _debuff = debuff;
        _amount_debuff = amount;

        if (!_is_casting)
        {
            SetIsCasting(true);
        }
    }

    public void hit_target()
    {
        if (!GodotObject.IsInstanceValid(_target))
        {
            return;
        }

        Enemy enemyModel = _target as Enemy;
        enemyModel?.apply_damage(_attack);
        if (_debuff != null)
        {
            enemyModel?.apply_debuff(_debuff, _amount_debuff);
        }
    }

    public void set_color(Color new_color)
    {
        _color = new_color;
        if (_line_2d != null)
        {
            _line_2d.Modulate = new_color;
        }
    }

    private void SetIsCasting(bool new_value)
    {
        if (_is_casting == new_value)
        {
            return;
        }

        _is_casting = new_value;
        if (!_is_casting)
        {
            Dissapear();
            return;
        }

        _current_laser_length = 0.0f;
        Appear();
    }

    private void Dissapear()
    {
        if (_red_attack_loop.IsInsideTree())
        {
            _red_attack_loop.Stop();
        }

        if (_red_attack_finish.IsInsideTree())
        {
            _red_attack_finish.Play();
        }

        if (_line_2d == null)
        {
            return;
        }

        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _fire_particles.Emitting = false;
        _tween = CreateTween();
        _tween.TweenProperty(_line_2d, "width", 0.0f, growth_time * 2.0f).FromCurrent();
        _tween.Finished += OnDisappearTweenFinished;
    }

    private void OnDisappearTweenFinished()
    {
        _line_2d?.Hide();
        _current_laser_length = 0.0f;
    }

    private async void Appear()
    {
        _red_attack_start.Play();
        if (_line_2d == null)
        {
            return;
        }

        _line_2d.Visible = true;
        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _fire_particles.Emitting = true;
        _tween = CreateTween();
        _tween.TweenProperty(_line_2d, "width", _line_width, growth_time * 2.0f).From(0.0f);

        await ToSignal(_red_attack_start, AudioStreamPlayer2D.SignalName.Finished);
        if (_is_casting)
        {
            _red_attack_loop.Play();
        }
    }
}
