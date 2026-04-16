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
        this._sparks = GetNode<GpuParticles2D>("%Sparks");
        this._flare = GetNode<GpuParticles2D>("%Flare");
        this._area2d = GetNode<Area2D>("%Area");
        this._line = GetNode<Line2D>("%Line");

        this._line.TopLevel = true;
        this._area2d.TopLevel = true;
    }

    public override void _Process(double delta)
    {
        if (!GodotObject.IsInstanceValid(this._target))
        {
            if (!this._hit)
            {
                QueueFree();
            }

            return;
        }

        if (this._hit)
        {
            return;
        }

        Enemy targetEnemy = this._target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy))
        {
            if (!this._hit)
            {
                QueueFree();
            }

            return;
        }

        this._end_global = targetEnemy.target_position;
        this._max_length = this._start_global.DistanceTo(this._end_global);

        this._current_length += this.extend_speed * (float)delta;
        this._current_length = Mathf.Min(this._current_length, this._max_length);

        Vector2 dir = (this._end_global - this._start_global).Normalized();
        Vector2 tipGlobal = this._start_global + dir * this._current_length;

        this._line.SetPointPosition(1, tipGlobal);
        this._area2d.GlobalPosition = tipGlobal;

        if (this._current_length >= this._max_length && !this._hit)
        {
            this._on_hit();
        }
    }

    public void set_target(Node2D target, Attack attack, int bounces)
    {
        this.max_bounces = bounces;
        this._target = target;
        this._attack = attack;
        this._start_global = this._end_global != Vector2.Zero ? this._end_global : GlobalPosition;
        this._end_global = this._target?.GlobalPosition ?? GlobalPosition;

        this._max_length = this._start_global.DistanceTo(this._end_global);
        this._current_length = 0.0f;
        this._hit = false;

        this._line.ClearPoints();
        this._line.AddPoint(this._start_global);
        this._line.AddPoint(this._start_global);

        this._flare.Visible = false;
        this._sparks.Visible = false;
    }

    private async void _on_hit()
    {
        this._hit = true;
        Enemy enemyModel = this._target as Enemy;
        enemyModel?.apply_damage(this._attack);
        this._flare.Visible = true;
        this._sparks.Visible = true;

        this._hit_enemies.Add(this._target);
        this._bounces_done += 1;

        await ToSignal(GetTree().CreateTimer(this.bounce_delay, false), Timer.SignalName.Timeout);
        this._try_bounce();
    }

    private void _try_bounce()
    {
        if (this._bounces_done >= this.max_bounces)
        {
            QueueFree();
            return;
        }

        Node2D nextEnemy = this._get_closest_valid_enemy();
        if (nextEnemy == null)
        {
            QueueFree();
            return;
        }

        this.set_target(nextEnemy, this._attack, this.max_bounces);
    }

    private Node2D _get_closest_valid_enemy()
    {
        Node2D closest = null;
        float minDist = float.PositiveInfinity;

        foreach (Node2D enemy in this.enemies_in_range)
        {
            if (!GodotObject.IsInstanceValid(enemy) || !enemy.IsInsideTree() || this._hit_enemies.Contains(enemy))
            {
                continue;
            }

            float d = enemy.GlobalPosition.DistanceSquaredTo(this._end_global);
            if (d < minDist)
            {
                minDist = d;
                closest = enemy;
            }
        }

        return closest;
    }

    private void _on_area_2d_body_exited(Node2D body)
    {
        this.enemies_in_range.Remove(body);
    }

    private void _on_area_2d_body_entered(Node2D body)
    {
        this.enemies_in_range.Add(body);
        body.TreeExited += () => this.enemies_in_range.Remove(body);
    }
}
