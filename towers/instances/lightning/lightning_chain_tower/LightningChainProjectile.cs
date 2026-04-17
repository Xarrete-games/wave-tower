using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LightningChainProjectile : Node2D
{
    [Export] public float extend_speed = 1500.0f;
    [Export] public int max_bounces = 3;
    [Export] public float bounce_delay = 0.1f;

    private readonly Array<Node2D> enemies_in_range = new();
    private readonly Array<Node2D> _hit_enemies = new();

    private Node2D _target;
    private float _current_length = 0.0f;
    private float _max_length = 0.0f;
    private bool _hit;
    private int _bounces_done;
    private Vector2 _start_global;
    private Vector2 _end_global;
    private Attack _attack;

    private GpuParticles2D _sparks;
    private GpuParticles2D _flare;
    private Area2D _area2d;
    private Line2D _line;

    public override void _Ready()
    {
        _sparks = GetNode<GpuParticles2D>("%Sparks");
        _flare = GetNode<GpuParticles2D>("%Flare");
        _area2d = GetNode<Area2D>("%Area");
        _line = GetNode<Line2D>("%Line");

        _line.TopLevel = true;
        _area2d.TopLevel = true;
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

        _end_global = targetEnemy.target_position;
        _max_length = _start_global.DistanceTo(_end_global);

        _current_length += extend_speed * (float)delta;
        _current_length = Mathf.Min(_current_length, _max_length);

        Vector2 dir = (_end_global - _start_global).Normalized();
        Vector2 tipGlobal = _start_global + dir * _current_length;

        _line.SetPointPosition(1, tipGlobal);
        _area2d.GlobalPosition = tipGlobal;

        if (_current_length >= _max_length && !_hit)
        {
            OnHit();
        }
    }

    public void set_target(Node2D target, Attack attack, int bounces)
    {
        max_bounces = bounces;
        _target = target;
        _attack = attack;
        _start_global = _end_global != Vector2.Zero ? _end_global : GlobalPosition;
        _end_global = _target?.GlobalPosition ?? GlobalPosition;

        _max_length = _start_global.DistanceTo(_end_global);
        _current_length = 0.0f;
        _hit = false;

        _line.ClearPoints();
        _line.AddPoint(_start_global);
        _line.AddPoint(_start_global);

        _flare.Visible = false;
        _sparks.Visible = false;
    }

    private async void OnHit()
    {
        _hit = true;
        Enemy enemyModel = _target as Enemy;
        enemyModel?.apply_damage(_attack);
        _flare.Visible = true;
        _sparks.Visible = true;

        _hit_enemies.Add(_target);
        _bounces_done += 1;

        await ToSignal(GetTree().CreateTimer(bounce_delay, false), Timer.SignalName.Timeout);
        TryBounce();
    }

    private void TryBounce()
    {
        if (_bounces_done >= max_bounces)
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

        set_target(nextEnemy, _attack, max_bounces);
    }

    private Node2D GetClosestValidEnemy()
    {
        Node2D closest = null;
        float minDist = float.PositiveInfinity;

        foreach (Node2D enemy in enemies_in_range)
        {
            if (!GodotObject.IsInstanceValid(enemy) || !enemy.IsInsideTree() || _hit_enemies.Contains(enemy))
            {
                continue;
            }

            float d = enemy.GlobalPosition.DistanceSquaredTo(_end_global);
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
        enemies_in_range.Remove(body);
    }

    private void OnArea2dBodyEntered(Node2D body)
    {
        enemies_in_range.Add(body);
        body.TreeExited += () => enemies_in_range.Remove(body);
    }
}
