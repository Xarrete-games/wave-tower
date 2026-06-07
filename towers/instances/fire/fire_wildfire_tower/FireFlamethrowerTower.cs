using Godot;

[GlobalClass]
public partial class FireFlamethrowerTower : TowerNode
{
    private Timer _flameThrowerDurationTimer;
    private FireFlamethrowerProjectile _fireFlamethrowerProjectile;

    public override void _Ready()
    {
        base._Ready();

        _flameThrowerDurationTimer = GetNode<Timer>("FlameThrowerDurationTimer");
        _fireFlamethrowerProjectile = GetNode<FireFlamethrowerProjectile>("FireFlamethrowerProjectile");

        TargetChanged += OnNewTargetChange;
        _flameThrowerDurationTimer.Timeout += OnFlameThrowerDurationTimerTimeout;
    }

    public override void _ExitTree()
    {
        TargetChanged -= OnNewTargetChange;
        if (_flameThrowerDurationTimer != null)
        {
            _flameThrowerDurationTimer.Timeout -= OnFlameThrowerDurationTimerTimeout;
        }

        base._ExitTree();
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget))
        {
            return;
        }

        _fireFlamethrowerProjectile.Fire();
        Attack attack = GetAttack();
        _fireFlamethrowerProjectile.SetTarget(_currentTarget, attack);
        _flameThrowerDurationTimer.Start();
    }

    private void OnNewTargetChange(Node2D enemy)
    {
        bool isThrowing = _fireFlamethrowerProjectile.IsThrowing();
        if (!isThrowing)
        {
            return;
        }

        if (!GodotObject.IsInstanceValid(enemy))
        {
            _fireFlamethrowerProjectile.Stop();
            return;
        }

        Attack attack = GetAttack();
        _fireFlamethrowerProjectile.SetTarget(enemy, attack);
    }

    private void OnFlameThrowerDurationTimerTimeout()
    {
        _fireFlamethrowerProjectile.Stop();
    }
}
