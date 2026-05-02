using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class FireLaserProjectiel : Node2D
{
    [Export] public int CastSpeed = 7000;
    [Export] public Color color = Colors.Red;

    [Export] public float GrowthTime = 0.1f;

    private Node2D _target;
    private Attack _attack;
    private EnemyDebuff _debuff;
    private int _amountDebuff = 1;
    private Tween _tween;
    private float _currentLaserLength;
    private bool _isCasting;

    private Line2D _line2D;
    private float _lineWidth;
    private AudioStreamPlayer2D _redAttackStart;
    private AudioStreamPlayer2D _redAttackLoop;
    private AudioStreamPlayer2D _redAttackFinish;
    private CpuParticles2D _fireParticles;

    private readonly float _amplitude = 10f;
    private readonly float _speed = -16f;
    private readonly float _phaseOffset = Mathf.Pi / 3f;
    private Color _color = Colors.Red;

    public override void _Ready()
    {
        _line2D = GetNode<Line2D>("Line2D");
        _lineWidth = _line2D.Width;
        _redAttackStart = GetNode<AudioStreamPlayer2D>("RedAttack_start");
        _redAttackLoop = GetNode<AudioStreamPlayer2D>("RedAttack_loop");
        _redAttackFinish = GetNode<AudioStreamPlayer2D>("RedAttack_finish");
        _fireParticles = GetNode<CpuParticles2D>("FireParticles");

        SetColor(color);
        _fireParticles.Emitting = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        Enemy targetEnemy = _target as Enemy;
        if (!_isCasting || !GodotObject.IsInstanceValid(targetEnemy))
        {
            return;
        }

        Vector2 targetPosition = targetEnemy.TargetPosition;
        LookAt(targetPosition);

        float distanceToTarget = GlobalPosition.DistanceTo(targetPosition);
        _fireParticles.GlobalPosition = targetPosition;

        _currentLaserLength = Mathf.MoveToward(_currentLaserLength, distanceToTarget, CastSpeed * (float)delta);

        int iMax = _line2D.GetPointCount() - 1;
        if (iMax <= 0)
        {
            return;
        }

        float time = Time.GetTicksMsec() / 1000.0f;
        for (int i = 0; i < _line2D.GetPointCount(); i++)
        {
            Vector2 pos = _line2D.GetPointPosition(i);
            pos.X = i * _currentLaserLength / iMax;
            pos.Y = Mathf.Sin(time * _speed + i * _phaseOffset) * _amplitude * Mathf.Sin(Mathf.Pi * i / iMax);
            _line2D.SetPointPosition(i, pos);
        }
    }

    public void Stop()
    {
        _target = null;
        SetIsCasting(false);
    }

    public void SetTarget(Node2D target, Attack attack)
    {
        SetTarget(target, attack, null, 1);
    }

    public void SetTarget(Node2D target, Attack attack, EnemyDebuff debuff)
    {
        SetTarget(target, attack, debuff, 1);
    }

    public void SetTarget(Node2D target, Attack attack, EnemyDebuff debuff, int amount)
    {
        if (target == _target)
        {
            return;
        }

        _target = target;
        _attack = attack;
        _debuff = debuff;
        _amountDebuff = amount;

        if (!_isCasting)
        {
            SetIsCasting(true);
        }
    }

    public void HitTarget()
    {
        if (!GodotObject.IsInstanceValid(_target))
        {
            return;
        }

        Enemy enemy = _target as Enemy;
        enemy?.ApplyDamage(_attack);
        if (_debuff != null)
        {
            enemy?.ApplyDebuff(_debuff, _amountDebuff);
        }
    }

    public void SetColor(Color newColor)
    {
        _color = newColor;
        if (_line2D != null)
        {
            _line2D.Modulate = newColor;
        }
    }

    private void SetIsCasting(bool newValue)
    {
        if (_isCasting == newValue)
        {
            return;
        }

        _isCasting = newValue;
        if (!_isCasting)
        {
            Disappear();
            return;
        }

        _currentLaserLength = 0.0f;
        Appear();
    }

    private void Disappear()
    {
        if (_redAttackLoop.IsInsideTree())
        {
            _redAttackLoop.Stop();
        }

        if (_redAttackFinish.IsInsideTree())
        {
            _redAttackFinish.Play();
        }

        if (_line2D == null)
        {
            return;
        }

        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _fireParticles.Emitting = false;
        _tween = CreateTween();
        _tween.TweenProperty(_line2D, "width", 0.0f, GrowthTime * 2.0f).FromCurrent();
        _tween.Finished += OnDisappearTweenFinished;
    }

    private void OnDisappearTweenFinished()
    {
        _line2D?.Hide();
        _currentLaserLength = 0.0f;
    }

    private void Appear()
    {
        AsyncTaskHelper.FireAndForget(AppearAsync(), "FireLaserProjectiel.AppearAsync");
    }

    private async Task AppearAsync()
    {
        _redAttackStart.Play();
        if (_line2D == null)
        {
            return;
        }

        _line2D.Visible = true;
        if (_tween != null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _fireParticles.Emitting = true;
        _tween = CreateTween();
        _tween.TweenProperty(_line2D, "width", _lineWidth, GrowthTime * 2.0f).From(0.0f);

        await ToSignal(_redAttackStart, AudioStreamPlayer2D.SignalName.Finished);
        if (_isCasting)
        {
            _redAttackLoop.Play();
        }
    }
}
