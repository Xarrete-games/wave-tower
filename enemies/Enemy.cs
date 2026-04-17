using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{
    public event Action<Enemy, Attack> die;
    public event Action<Enemy> target_reached;

    public enum TypeLegacy
    {
        SPECTRE,
        BUBA,
        BIG_SPECTRE,
        GOLEM,
        SKELETON,
        BLACK_GOLEM,
        BLACK_SKELETON,
        GOLD_SKELETON,
        INVOKER,
        SKULL,
    }

    private static readonly PackedScene GOLD_DROPPED = GD.Load<PackedScene>("uid://cxs4ar5enx4mn");
    private static readonly PackedScene DAMAGE_NUMBERS = GD.Load<PackedScene>("uid://bkiu4qgh3ug1m");
    private const float WAYPOINT_ARRIVAL_THRESHOLD = 8.0f;
    private const float STEERING_FACTOR = 8.0f;

    [Export] public float base_speed = 80.0f;
    [Export] public float max_health = 50.0f;
    [Export] public int gold_value = 1;
    [Export] public int damage = 1;

    public float health;
    public Tween hit_tween;
    public bool _last_is_right_direction = false;

    public Color default_modulate_color = Colors.White;
    public float _base_speed = 100.0f;
    public float _speed_mult = 1.0f;
    public bool is_right_direction = true;

    public bool enabled = true;
    public bool _is_dead = false;

    private readonly Array<Vector2> _waypoints = new();
    private int _current_waypoint_index = 0;
    private float _total_path_length = 0.0f;
    private Vector2 _velocity = Vector2.Zero;

    private AnimationPlayer animation_player;
    public HealthBar health_bar;
    private AnimatedSprite2D animated_sprite_2d;
    private CollisionShape2D collision_shape_2d;
    public DebuffHandler debuff_handler;
    private Marker2D target_position_left;
    private Marker2D target_position_right;

    public float speed
    {
        get => _base_speed * _speed_mult;
        set => _base_speed = value;
    }

    public float speed_mult
    {
        get => _speed_mult;
        set => _speed_mult = value;
    }

    public Vector2 target_position => is_right_direction ? target_position_right.GlobalPosition : target_position_left.GlobalPosition;

    public Vector2 inversed_target_position => is_right_direction ? target_position_left.GlobalPosition : target_position_right.GlobalPosition;

    public override async void _Ready()
    {
        animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
        health_bar = GetNode<HealthBar>("HealthBar");
        animated_sprite_2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        collision_shape_2d = GetNode<CollisionShape2D>("CollisionShape2D");
        debuff_handler = GetNode<DebuffHandler>("DebuffHandler");
        target_position_left = GetNode<Marker2D>("TargetPositionLeft");
        target_position_right = GetNode<Marker2D>("TargetPositionRight");

        speed = base_speed;
        health_bar.set_max_health(max_health);
        _set_health(max_health);

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);
        enabled = true;
    }

    public override void _Process(double delta)
    {
        debuff_handler.update_all(this);

        if (_waypoints.Count > 0)
        {
            _process_waypoints((float)delta);
        }
    }

    public void _process_waypoints(float delta)
    {
        if (_current_waypoint_index >= _waypoints.Count)
        {
            _on_target_reached();
            return;
        }

        Vector2 targetPoint = _waypoints[_current_waypoint_index];
        float distance = GlobalPosition.DistanceTo(targetPoint);

        if (distance <= WAYPOINT_ARRIVAL_THRESHOLD)
        {
            _current_waypoint_index += 1;
            return;
        }

        float previousGlobalX = GlobalPosition.X;
        float previousGlobalY = GlobalPosition.Y;

        Vector2 direction = (targetPoint - GlobalPosition).Normalized();
        Vector2 desiredVelocity = direction * speed;
        _velocity = _velocity.MoveToward(desiredVelocity, STEERING_FACTOR * speed * delta);

        Vector2 displacement = _velocity * delta;
        if (displacement.Length() >= distance)
        {
            GlobalPosition = targetPoint;
        }
        else
        {
            GlobalPosition += displacement;
        }

        _update_sprite_direction(previousGlobalX);
        _update_animation(previousGlobalY);
    }

    public void _update_sprite_direction(float previous_x)
    {
        is_right_direction = GlobalPosition.X > previous_x;
        Marker2D marker = is_right_direction ? target_position_right : target_position_left;

        Vector2 hbPos = health_bar.Position;
        hbPos.X = marker.Position.X - health_bar.Size.X * health_bar.Scale.X * 0.5f;
        health_bar.Position = hbPos;

        if (is_right_direction != _last_is_right_direction)
        {
            animated_sprite_2d.FlipH = !animated_sprite_2d.FlipH;
            _last_is_right_direction = is_right_direction;
        }
    }

    public void _update_animation(float previous_y)
    {
        string animation = previous_y > GlobalPosition.Y ? "top right" : "down right";
        if (animated_sprite_2d.Animation != animation || !animated_sprite_2d.IsPlaying())
        {
            animated_sprite_2d.Play(animation);
        }
    }

    public void set_waypoints(Array<Vector2> waypoints)
    {
        _waypoints.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            _waypoints.Add(waypoints[i]);
        }

        _current_waypoint_index = 0;
        _velocity = Vector2.Zero;
        _total_path_length = _compute_path_length(_waypoints);
    }

    public float _compute_path_length(Array<Vector2> points)
    {
        float length = 0.0f;
        for (int i = 1; i < points.Count; i++)
        {
            length += points[i - 1].DistanceTo(points[i]);
        }

        return length;
    }

    public float get_progress_ratio()
    {
        if (_waypoints.Count == 0 || _total_path_length <= 0.0f)
        {
            return 0.0f;
        }

        if (_current_waypoint_index >= _waypoints.Count)
        {
            return 1.0f;
        }

        float covered = 0.0f;
        for (int i = 1; i < _current_waypoint_index; i++)
        {
            covered += _waypoints[i - 1].DistanceTo(_waypoints[i]);
        }

        Vector2 segStart = _current_waypoint_index > 0 ? _waypoints[_current_waypoint_index - 1] : _waypoints[0];
        covered += segStart.DistanceTo(GlobalPosition);
        return Mathf.Clamp(covered / _total_path_length, 0.0f, 1.0f);
    }

    public bool has_waypoints()
    {
        return _waypoints.Count > 0 && _current_waypoint_index < _waypoints.Count;
    }

    public Vector2 get_current_waypoint()
    {
        if (_current_waypoint_index < _waypoints.Count)
        {
            return _waypoints[_current_waypoint_index];
        }

        return Vector2.Zero;
    }

    public void disable()
    {
        enabled = false;
        animated_sprite_2d.Visible = false;
        collision_shape_2d.Disabled = true;
        health_bar.Visible = false;
    }

    public async void enable()
    {
        enabled = true;
        animated_sprite_2d.Visible = true;
        health_bar.Visible = true;

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        collision_shape_2d.Disabled = false;
    }

    public float get_percentage_remaining_health()
    {
        if (max_health <= 0.0f)
        {
            return 0.0f;
        }

        float healthRatio = health / max_health;
        float percentage = healthRatio * 100.0f;
        return Mathf.Min(100.0f, percentage);
    }

    public float get_remaining_health()
    {
        return health;
    }

    public int get_debuff_stacks(int debuff_type)
    {
        return debuff_handler.get_stacks(debuff_type);
    }

    public System.Collections.Generic.List<EnemyDebuff> get_active_debuffs()
    {
        return debuff_handler.get_active_debuffs();
    }

    public bool has_any_debuff()
    {
        return debuff_handler.has_any_defbuff();
    }

    public void apply_debuff(EnemyDebuff debuff, int amount = 1)
    {
        debuff_handler.add_debuff(debuff, amount, this);
    }

    public void apply_damage(Attack attack)
    {
        if (_is_dead)
        {
            return;
        }

        DamageContext ctx = new(BuildAttackModel(attack), BuildEnemyModel());
        Hooks.OnBeforeDamage(Hooks.GetListenersFromRuntime(), ctx);
        float modifiedDamage = ctx.get_total_damage();

        attack.damage = modifiedDamage;

        _set_health(health - attack.damage);
        _play_hit_animation();
        _show_damage(attack);

        if (health <= 0.0f && !_is_dead)
        {
            _is_dead = true;
            _die(attack);
        }
    }

    public async void _play_hit_animation()
    {
        if (hit_tween != null && hit_tween.IsRunning())
        {
            hit_tween.Kill();
        }

        hit_tween = CreateTween();
        hit_tween.TweenProperty(animated_sprite_2d, "modulate", Colors.Red, 0.2f);
        await ToSignal(hit_tween, Tween.SignalName.Finished);
        animated_sprite_2d.Modulate = default_modulate_color;
    }

    public void _die(Attack attack)
    {
        die?.Invoke(this, attack);
        Hooks.OnEnemyDie(Hooks.GetListenersFromRuntime(), BuildEnemyModel(), BuildAttackModel(attack));
        _show_gold_dropped();

        RunContext runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext?.economy != null)
        {
            runContext.economy.gold += gold_value;
        }

        QueueFree();
    }

    private AttackModel BuildAttackModel(Attack attack)
    {
        var model = new AttackModel
        {
            Damage = attack?.damage ?? 0f,
            CritChance = attack?.crit_chance ?? 0f,
            IsCritical = attack?.is_critical ?? false,
            IsExecution = attack?.is_execution ?? false,
            Hits = attack?.hits ?? 1,
            Bounces = attack?.bounces ?? 0,
        };

        Source source = attack?.source;
        model.Source = new SourceModel
        {
            Type = source?.Type switch
            {
                Source.SourceType.RELIC => SourceModel.SourceType.Relic,
                Source.SourceType.TOWER => SourceModel.SourceType.Tower,
                Source.SourceType.CONSUMABLE => SourceModel.SourceType.Consumable,
                Source.SourceType.DEBUFF => SourceModel.SourceType.Debuff,
                _ => SourceModel.SourceType.Global,
            },
            TypeId = source?.TypeId ?? string.Empty,
        };

        return model;
    }

    private EnemyModel BuildEnemyModel()
    {
        return new EnemyModel
        {
            MaxHealth = max_health,
            RemainingHealth = health,
            ProgressRatio = get_progress_ratio(),
            GoldValue = gold_value,
            HasAnyDebuff = has_any_debuff(),
        };
    }

    public void _show_damage(Attack attack)
    {
        const int MAX_OFFSET = 30;
        DamageNumbers damageNumbers = DAMAGE_NUMBERS.Instantiate<DamageNumbers>();
        Vector2 basePosition = target_position;

        int randomOffsetX = (int)GD.RandRange(-MAX_OFFSET, MAX_OFFSET);
        int randomOffsetY = (int)GD.RandRange(-MAX_OFFSET, MAX_OFFSET);

        damageNumbers.GlobalPosition = basePosition + new Vector2(randomOffsetX, randomOffsetY);

        GetTree().Root.AddChild(damageNumbers);
        damageNumbers.set_attack(attack);
    }

    public void _show_gold_dropped()
    {
        GoldDropped goldDropped = GOLD_DROPPED.Instantiate<GoldDropped>();
        GetTree().Root.AddChild(goldDropped);
        goldDropped.set_gold(gold_value);
        goldDropped.GlobalPosition = target_position;
    }

    public void _on_target_reached()
    {
        target_reached?.Invoke(this);
        QueueFree();
    }

    public void _set_health(float new_value)
    {
        health = new_value;
        health_bar.update_health(health);
    }
}
