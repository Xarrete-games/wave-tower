using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{
    [Signal]
    public delegate void dieEventHandler(Variant enemy, Variant attack);

    [Signal]
    public delegate void target_reachedEventHandler(Variant enemy);

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
        get => this._base_speed * this._speed_mult;
        set => this._base_speed = value;
    }

    public float speed_mult
    {
        get => this._speed_mult;
        set => this._speed_mult = value;
    }

    public Vector2 target_position => this.is_right_direction ? this.target_position_right.GlobalPosition : this.target_position_left.GlobalPosition;

    public Vector2 inversed_target_position => this.is_right_direction ? this.target_position_left.GlobalPosition : this.target_position_right.GlobalPosition;

    public override async void _Ready()
    {
        this.animation_player = GetNode<AnimationPlayer>("AnimationPlayer");
        this.health_bar = GetNode<HealthBar>("HealthBar");
        this.animated_sprite_2d = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        this.collision_shape_2d = GetNode<CollisionShape2D>("CollisionShape2D");
        this.debuff_handler = GetNode<DebuffHandler>("DebuffHandler");
        this.target_position_left = GetNode<Marker2D>("TargetPositionLeft");
        this.target_position_right = GetNode<Marker2D>("TargetPositionRight");

        this.speed = this.base_speed;
        this.health_bar.set_max_health(this.max_health);
        this._set_health(this.max_health);

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);
        this.enabled = true;
    }

    public override void _Process(double delta)
    {
        this.debuff_handler.update_all(this);

        if (this._waypoints.Count > 0)
        {
            this._process_waypoints((float)delta);
        }
    }

    public void _process_waypoints(float delta)
    {
        if (this._current_waypoint_index >= this._waypoints.Count)
        {
            this._on_target_reached();
            return;
        }

        Vector2 targetPoint = this._waypoints[this._current_waypoint_index];
        float distance = GlobalPosition.DistanceTo(targetPoint);

        if (distance <= WAYPOINT_ARRIVAL_THRESHOLD)
        {
            this._current_waypoint_index += 1;
            return;
        }

        float previousGlobalX = GlobalPosition.X;
        float previousGlobalY = GlobalPosition.Y;

        Vector2 direction = (targetPoint - GlobalPosition).Normalized();
        Vector2 desiredVelocity = direction * this.speed;
        this._velocity = this._velocity.MoveToward(desiredVelocity, STEERING_FACTOR * this.speed * delta);

        Vector2 displacement = this._velocity * delta;
        if (displacement.Length() >= distance)
        {
            GlobalPosition = targetPoint;
        }
        else
        {
            GlobalPosition += displacement;
        }

        this._update_sprite_direction(previousGlobalX);
        this._update_animation(previousGlobalY);
    }

    public void _update_sprite_direction(float previous_x)
    {
        this.is_right_direction = GlobalPosition.X > previous_x;
        Marker2D marker = this.is_right_direction ? this.target_position_right : this.target_position_left;

        Vector2 hbPos = this.health_bar.Position;
        hbPos.X = marker.Position.X - this.health_bar.Size.X * this.health_bar.Scale.X * 0.5f;
        this.health_bar.Position = hbPos;

        if (this.is_right_direction != this._last_is_right_direction)
        {
            this.animated_sprite_2d.FlipH = !this.animated_sprite_2d.FlipH;
            this._last_is_right_direction = this.is_right_direction;
        }
    }

    public void _update_animation(float previous_y)
    {
        string animation = previous_y > GlobalPosition.Y ? "top right" : "down right";
        if (this.animated_sprite_2d.Animation != animation || !this.animated_sprite_2d.IsPlaying())
        {
            this.animated_sprite_2d.Play(animation);
        }
    }

    public void set_waypoints(Array<Vector2> waypoints)
    {
        this._waypoints.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            this._waypoints.Add(waypoints[i]);
        }

        this._current_waypoint_index = 0;
        this._velocity = Vector2.Zero;
        this._total_path_length = this._compute_path_length(this._waypoints);
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
        if (this._waypoints.Count == 0 || this._total_path_length <= 0.0f)
        {
            return 0.0f;
        }

        if (this._current_waypoint_index >= this._waypoints.Count)
        {
            return 1.0f;
        }

        float covered = 0.0f;
        for (int i = 1; i < this._current_waypoint_index; i++)
        {
            covered += this._waypoints[i - 1].DistanceTo(this._waypoints[i]);
        }

        Vector2 segStart = this._current_waypoint_index > 0 ? this._waypoints[this._current_waypoint_index - 1] : this._waypoints[0];
        covered += segStart.DistanceTo(GlobalPosition);
        return Mathf.Clamp(covered / this._total_path_length, 0.0f, 1.0f);
    }

    public bool has_waypoints()
    {
        return this._waypoints.Count > 0 && this._current_waypoint_index < this._waypoints.Count;
    }

    public Vector2 get_current_waypoint()
    {
        if (this._current_waypoint_index < this._waypoints.Count)
        {
            return this._waypoints[this._current_waypoint_index];
        }

        return Vector2.Zero;
    }

    public void disable()
    {
        this.enabled = false;
        this.animated_sprite_2d.Visible = false;
        this.collision_shape_2d.Disabled = true;
        this.health_bar.Visible = false;
    }

    public async void enable()
    {
        this.enabled = true;
        this.animated_sprite_2d.Visible = true;
        this.health_bar.Visible = true;

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        this.collision_shape_2d.Disabled = false;
    }

    public float get_percentage_remaining_health()
    {
        if (this.max_health <= 0.0f)
        {
            return 0.0f;
        }

        float healthRatio = this.health / this.max_health;
        float percentage = healthRatio * 100.0f;
        return Mathf.Min(100.0f, percentage);
    }

    public float get_remaining_health()
    {
        return this.health;
    }

    public int get_debuff_stacks(int debuff_type)
    {
        return this.debuff_handler.get_stacks(debuff_type);
    }

    public Array<EnemyDebuff> get_active_debuffs()
    {
        return this.debuff_handler.get_active_debuffs();
    }

    public bool has_any_debuff()
    {
        return this.debuff_handler.has_any_defbuff();
    }

    public void apply_debuff(EnemyDebuff debuff, int amount = 1)
    {
        this.debuff_handler.add_debuff(debuff, amount, this);
    }

    public void apply_damage(Attack attack)
    {
        if (this._is_dead)
        {
            return;
        }

        DamageContext ctx = new(attack, this);
        Hooks.on_before_damage(ctx);
        float modifiedDamage = ctx.get_total_damage();

        attack.damage = modifiedDamage;

        this._set_health(this.health - attack.damage);
        this._play_hit_animation();
        this._show_damage(attack);

        if (this.health <= 0.0f && !this._is_dead)
        {
            this._is_dead = true;
            this._die(attack);
        }
    }

    public async void _play_hit_animation()
    {
        if (this.hit_tween != null && this.hit_tween.IsRunning())
        {
            this.hit_tween.Kill();
        }

        this.hit_tween = CreateTween();
        this.hit_tween.TweenProperty(this.animated_sprite_2d, "modulate", Colors.Red, 0.2f);
        await ToSignal(this.hit_tween, Tween.SignalName.Finished);
        this.animated_sprite_2d.Modulate = this.default_modulate_color;
    }

    public void _die(Attack attack)
    {
        EmitSignal(SignalName.die, this, attack);
        Hooks.on_enemy_die(this, attack);
        this._show_gold_dropped();

        RunContext runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext?.economy != null)
        {
            runContext.economy.gold += this.gold_value;
        }

        QueueFree();
    }

    public void _show_damage(Attack attack)
    {
        const int MAX_OFFSET = 30;
        DamageNumbers damageNumbers = DAMAGE_NUMBERS.Instantiate<DamageNumbers>();
        Vector2 basePosition = this.target_position;

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
        goldDropped.set_gold(this.gold_value);
        goldDropped.GlobalPosition = this.target_position;
    }

    public void _on_target_reached()
    {
        EmitSignal(SignalName.target_reached, this);
        QueueFree();
    }

    public void _set_health(float new_value)
    {
        this.health = new_value;
        this.health_bar.update_health(this.health);
    }
}
