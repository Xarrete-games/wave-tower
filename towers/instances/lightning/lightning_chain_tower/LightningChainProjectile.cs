using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LightningChainProjectile : Node2D
{
    [Export] public float ExtendSpeed = 1500.0f;
    [Export] public int MaxBounces = 3;
    [Export] public float BounceDelay = 0.1f;

    private readonly Array<Node2D> _enemiesInRange = new();
    private readonly Array<Node2D> _hitEnemies = new();

    private Node2D _target;
    private float _currentLength;
    private float _maxLength;
    private bool _hit;
    private int _bouncesDone;
    private Vector2 _startGlobal;
    private Vector2 _endGlobal;
    private Attack _attack;

    private GpuParticles2D _sparks;
    private GpuParticles2D _flare;
    private Area2D _area2D;
    private Line2D _line;

    public override void _Ready()
    {
        _sparks = GetNode<GpuParticles2D>("%Sparks");
        _flare = GetNode<GpuParticles2D>("%Flare");
        _area2D = GetNode<Area2D>("%Area");
        _line = GetNode<Line2D>("%Line");

        _line.TopLevel = true;
        _area2D.TopLevel = true;
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(_target))
        {
            if (!_hit)
            {
                QueueFree();
            }

            return;
        }

        if (_hit)
        {
            return;
        }

        Enemy targetEnemy = _target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy))
        {
            if (!_hit)
            {
                QueueFree();
            }

            return;
        }

        _endGlobal = targetEnemy.TargetPosition;
        _maxLength = _startGlobal.DistanceTo(_endGlobal);

        _currentLength += ExtendSpeed * (float)delta;
        _currentLength = Mathf.Min(_currentLength, _maxLength);

        Vector2 dir = (_endGlobal - _startGlobal).Normalized();
        Vector2 tipGlobal = _startGlobal + dir * _currentLength;

        _line.SetPointPosition(1, tipGlobal);
        _area2D.GlobalPosition = tipGlobal;

        if (_currentLength >= _maxLength && !_hit)
        {
            OnHit();
        }
    }

    public void SetTarget(Node2D target, Attack attack, int bounces)
    {
        MaxBounces = bounces;
        _target = target;
        _attack = attack;
        _startGlobal = _endGlobal != Vector2.Zero ? _endGlobal : GlobalPosition;
        _endGlobal = _target?.GlobalPosition ?? GlobalPosition;

        _maxLength = _startGlobal.DistanceTo(_endGlobal);
        _currentLength = 0.0f;
        _hit = false;

        _line.ClearPoints();
        _line.AddPoint(_startGlobal);
        _line.AddPoint(_startGlobal);

        _flare.Visible = false;
        _sparks.Visible = false;
    }

    private async void OnHit()
    {
        _hit = true;
        Enemy enemy = _target as Enemy;
        enemy?.ApplyDamage(_attack);
        _flare.Visible = true;
        _sparks.Visible = true;

        _hitEnemies.Add(_target);
        _bouncesDone += 1;

        await ToSignal(GetTree().CreateTimer(BounceDelay, false), Timer.SignalName.Timeout);
        TryBounce();
    }

    private void TryBounce()
    {
        if (_bouncesDone >= MaxBounces)
        {
            QueueFree();
            return;
        }

        Node2D nextEnemy = GetClosestValidEnemy();
        if (nextEnemy == null)
        {
            QueueFree();
            return;
        }

        SetTarget(nextEnemy, _attack, MaxBounces);
    }

    private Node2D GetClosestValidEnemy()
    {
        Node2D closest = null;
        float minDist = float.PositiveInfinity;

        foreach (Node2D enemy in _enemiesInRange)
        {
            if (!GodotObject.IsInstanceValid(enemy) || !enemy.IsInsideTree() || _hitEnemies.Contains(enemy))
            {
                continue;
            }

            float d = enemy.GlobalPosition.DistanceSquaredTo(_endGlobal);
            if (d < minDist)
            {
                minDist = d;
                closest = enemy;
            }
        }

        return closest;
    }

    private void OnArea2dBodyExited(Node2D body)
    {
        _enemiesInRange.Remove(body);
    }

    private void OnArea2dBodyEntered(Node2D body)
    {
        _enemiesInRange.Add(body);
        body.TreeExited += () => _enemiesInRange.Remove(body);
    }
}
