using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class Tower : Node2D
{
    [Signal]
    public delegate void stats_changeEventHandler(Variant tower);

    [Signal]
    public delegate void attack_firedEventHandler();

    [Signal]
    public delegate void on_target_changeEventHandler(Node2D enemy);

    [Signal]
    public delegate void buff_addedEventHandler(Variant buff);

    [Signal]
    public delegate void buff_removedEventHandler(Variant buff);

    public enum Type
    {
        FIRE,
        LIGHTNING,
        FROST,
    }

    public enum TargetingMode
    {
        FIRST_IN_PROGRESS,
        HIGH_HP,
        LOW_HP,
    }

    public const int SOURCE_TYPE_TOWER = 1;
    public const int MAX_LEVEL = 2;
    public static readonly Color PHANTOM_COLOR = new(1f, 1f, 1f, 0.5f);

    public const float ELLIPSE_Y_RATIO = 0.5f;
    public const float TOWER_AREA_RADIUS = 47.042534f;

    [Export] public Type type = Type.FIRE;

    public GodotObject data;
    public int build_price = 0;

    protected Node2D _current_target;
    protected bool _enabled = false;
    protected bool _first_shot = true;

    public Tower current_tower_selected;
    public Tween range_tween;

    private GodotObject _stats;
    public GodotObject stats
    {
        get => this._stats;
        set
        {
            this._stats = value;
            EmitSignal(SignalName.stats_change, this);
        }
    }

    public Vector2I tile_pos;
    public string composite_tile_key = string.Empty;

    private int _targeting_mode = (int)TargetingMode.FIRST_IN_PROGRESS;
    public int targeting_mode
    {
        get => this._targeting_mode;
        set
        {
            this._targeting_mode = value;
            if (this.area_detector != null)
            {
                this.area_detector.targeting_type = value;
            }
        }
    }

    public int level = 1;

    private GodotObject _exp_data;
    public GodotObject exp_data
    {
        get => this._exp_data;
        set
        {
            this._exp_data = value;
            EmitSignal(SignalName.stats_change, this);
        }
    }

    public string id = string.Empty;
    public string type_id = string.Empty;

    public readonly Array<GodotObject> buffs = new();

    public TowerLogic tower_logic;

    public Source damage_source
    {
        get
        {
            Source src = new();
            src.setup(SOURCE_TYPE_TOWER, this.type_id, this);
            return src;
        }
    }

    protected AreaDetector area_detector;
    protected RangePreview range_preview;
    protected CollisionPolygon2D range_collision;
    protected Control mouse_detector;
    protected Timer attack_timer;
    protected CristalLight cristal_light;
    protected Sprite2D sprite_2d;
    protected Node tower_stats_handler;
    protected Area2D tower_area;
    protected CollisionPolygon2D tower_area_collision;

    private bool _eventsConnected;
    private Callable _towerSelectedCallable;

    public override void _Ready()
    {
        this.area_detector = GetNode<AreaDetector>("AreaDetector");
        this.range_preview = GetNode<RangePreview>("RangePreview");
        this.range_collision = GetNode<CollisionPolygon2D>("AreaDetector/RangeCollision");
        this.mouse_detector = GetNode<Control>("MouseDetector");
        this.attack_timer = GetNode<Timer>("AttackTimer");
        this.cristal_light = GetNodeOrNull<CristalLight>("CristalLight");
        this.sprite_2d = GetNodeOrNull<Sprite2D>("Sprite2D");
        this.tower_stats_handler = GetNode("TowerStatsHandler");
        this.tower_area = GetNode<Area2D>("TowerArea");
        this.tower_area_collision = GetNode<CollisionPolygon2D>("TowerArea/CollisionShape2D");

        this.type_id = GetType().Name;

        this.data?.Call("build");
        this.tower_logic = new TowerLogic(this);
        this.tower_area_collision.Polygon = build_ellipse_polygon(TOWER_AREA_RADIUS, TOWER_AREA_RADIUS * ELLIPSE_Y_RATIO);

        this.tower_stats_handler.Connect("stats_change", Callable.From<Variant>(this._on_stats_change));
        this.tower_stats_handler.Connect("buff_applied", Callable.From<Variant>(this.add_buff));
        this.tower_stats_handler.Connect("buff_expired", Callable.From<string>(this.remove_buff));
        this.tower_stats_handler.Call("set_data", this.data, (int)this.type);

        this.area_detector.target_change += this._on_target_change;
        this.targeting_mode = this._targeting_mode;
    }

    public override void _ExitTree()
    {
        if (this.area_detector != null)
        {
            this.area_detector.target_change -= this._on_target_change;
        }

        if (this._eventsConnected)
        {
            GodotObject towersManager = GetTowersManager();
            if (towersManager != null && !this._towerSelectedCallable.Equals(default(Callable)) && towersManager.IsConnected("tower_selected", this._towerSelectedCallable))
            {
                towersManager.Disconnect("tower_selected", this._towerSelectedCallable);
            }

            this.mouse_detector.GuiInput -= this._on_gui_input;
            this.mouse_detector.MouseEntered -= this._on_mouse_entered;
            this.mouse_detector.MouseExited -= this._on_mouse_exit;
            this._eventsConnected = false;
        }
    }

    public static Vector2[] build_ellipse_polygon(float radius_x, float radius_y, int segments = 48)
    {
        Vector2[] points = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Tau * i / (float)segments;
            points[i] = new Vector2(Mathf.Cos(angle) * radius_x, Mathf.Sin(angle) * radius_y);
        }

        return points;
    }

    public static string targeting_mode_to_string(int mode)
    {
        return mode switch
        {
            (int)TargetingMode.FIRST_IN_PROGRESS => "Progress",
            (int)TargetingMode.HIGH_HP => "High Health",
            (int)TargetingMode.LOW_HP => "Low Health",
            _ => "Unknown",
        };
    }

    public void phantom_mode()
    {
        if (this.sprite_2d != null)
        {
            this.sprite_2d.Modulate = PHANTOM_COLOR;
        }

        if (this.range_preview != null)
        {
            this.range_preview.Visible = false;
        }
    }

    public void normal_color()
    {
        if (this.sprite_2d != null)
        {
            this.sprite_2d.Modulate = Colors.White;
        }

        this._show_range();
    }

    public virtual void placement_mode()
    {
        this._enabled = false;
        this.area_detector.Monitoring = false;
        this.tower_area.Monitorable = false;
    }

    public virtual async void enable()
    {
        if (this.sprite_2d != null)
        {
            this.sprite_2d.Modulate = Colors.White;
        }

        this._enabled = true;
        this.area_detector.Monitoring = true;
        this.tower_area.Monitorable = true;

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        if (this._eventsConnected)
        {
            return;
        }

        GodotObject towersManager = GetTowersManager();
        if (towersManager != null)
        {
            this._towerSelectedCallable = Callable.From<Variant>(this._on_tower_selected);
            towersManager.Connect("tower_selected", this._towerSelectedCallable);
        }

        this.mouse_detector.GuiInput += this._on_gui_input;
        this.mouse_detector.MouseEntered += this._on_mouse_entered;
        this.mouse_detector.MouseExited += this._on_mouse_exit;
        this._eventsConnected = true;
    }

    public virtual void add_buff(Variant tower_buff_var)
    {
        GodotObject tower_buff = tower_buff_var.AsGodotObject();
        if (tower_buff == null)
        {
            return;
        }

        this.buffs.Add(tower_buff);
        if (tower_buff.Get("value").VariantType != Variant.Type.Nil)
        {
            this.tower_stats_handler.Call("add_buff", tower_buff);
        }

        EmitSignal(SignalName.buff_added, tower_buff);
    }

    public virtual void remove_buff(string source_id)
    {
        List<GodotObject> removedBuffs = new();

        for (int i = this.buffs.Count - 1; i >= 0; i--)
        {
            GodotObject buff = this.buffs[i];
            if (buff == null)
            {
                continue;
            }

            GodotObject source = buff.Get("source").AsGodotObject();
            string buffSourceId = source?.Get("type_id").AsString() ?? string.Empty;
            if (buffSourceId != source_id)
            {
                continue;
            }

            removedBuffs.Add(buff);
            this.buffs.RemoveAt(i);
        }

        this.tower_stats_handler.Call("remove_buff", source_id);

        foreach (GodotObject removedBuff in removedBuffs)
        {
            EmitSignal(SignalName.buff_removed, removedBuff);
        }
    }

    public virtual void upgrade()
    {
        GodotObject runContext = GetRunContext();
        GodotObject economy = runContext?.Get("economy").AsGodotObject();
        if (economy != null && this.data != null)
        {
            int currentGold = economy.Get("gold").AsInt32();
            int upgradePrice = this.data.Get("upgrade_price").AsInt32();
            economy.Set("gold", currentGold - upgradePrice);
        }

        this.level += 1;
        this.tower_stats_handler.Call("level_up", this.level);
    }

    public bool is_max_level()
    {
        return this.level >= MAX_LEVEL;
    }

    public void copy_tower_data(Variant from_tower_var)
    {
        Tower from_tower = from_tower_var.AsGodotObject() as Tower;
        if (from_tower == null)
        {
            return;
        }

        this.area_detector.targets_in_range.Clear();
        foreach (Node2D enemy in from_tower.area_detector.targets_in_range)
        {
            this.area_detector.targets_in_range.Add(enemy);
        }

        this.area_detector.current_target = from_tower.area_detector.current_target;
        this.targeting_mode = from_tower.targeting_mode;
    }

    public GodotObject _get_attack()
    {
        bool is_critical = this._is_critical_hit();

        float baseDamage = this.stats?.Get("damage").AsSingle() ?? 0f;
        float critDamage = this.stats?.Get("critic_damage").AsSingle() ?? 0f;
        float attackDamage = is_critical ? baseDamage * (1f + critDamage / 100f) : baseDamage;

        Attack attack = new(attackDamage, this.damage_source, is_critical);
        Enemy targetEnemy = this._current_target as Enemy;
        AttackContext ctx = new(this.BuildEnemyModel(targetEnemy), this.BuildAttackModel(attack), this.BuildTowerModel());
        Hooks.OnBeforeAttack(Hooks.GetListenersFromRuntime(), ctx);
        attack.damage = ctx.rebuild_attack();

        return attack;
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
            Type = source?.type switch
            {
                Source.SourceType.RELIC => SourceModel.SourceType.Relic,
                Source.SourceType.TOWER => SourceModel.SourceType.Tower,
                Source.SourceType.CONSUMABLE => SourceModel.SourceType.Consumable,
                Source.SourceType.DEBUFF => SourceModel.SourceType.Debuff,
                _ => SourceModel.SourceType.Global,
            },
            TypeId = source?.type_id ?? string.Empty,
        };

        return model;
    }

    private EnemyModel BuildEnemyModel(Enemy enemy)
    {
        if (enemy == null)
        {
            return new EnemyModel();
        }

        return new EnemyModel
        {
            MaxHealth = enemy.max_health,
            RemainingHealth = enemy.health,
            ProgressRatio = enemy.get_progress_ratio(),
            GoldValue = enemy.gold_value,
            HasAnyDebuff = enemy.has_any_debuff(),
        };
    }

    private TowerModel BuildTowerModel()
    {
        TowerModel.TowerType towerType = this.type switch
        {
            Type.FIRE => TowerModel.TowerType.Fire,
            Type.LIGHTNING => TowerModel.TowerType.Lightning,
            Type.FROST => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        return new TowerModel
        {
            Id = this.id,
            TypeId = this.type_id,
            Type = towerType,
        };
    }

    public bool _is_critical_hit()
    {
        float randomValue = GD.Randf();
        float critChance = this.stats?.Get("critic_chance").AsSingle() ?? 0f;
        return randomValue < (critChance / 100f);
    }

    private void _on_target_change(Node2D enemy)
    {
        this._current_target = enemy;
        EmitSignal(SignalName.on_target_change, enemy);

        if (this._first_shot && GodotObject.IsInstanceValid(this._current_target))
        {
            this._fire();
            EmitSignal(SignalName.attack_fired);
            this.attack_timer.Start();
            this._first_shot = false;
        }
    }

    private void _on_attack_timer_timeout()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || !this._enabled)
        {
            this._first_shot = true;
            return;
        }

        this._fire();
        EmitSignal(SignalName.attack_fired);
    }

    protected virtual void _fire() { }

    public virtual void _on_stats_change(Variant new_stats)
    {
        this.stats = new_stats.AsGodotObject();
        this._apply_stats_changes();
    }

    public virtual void _apply_stats_changes()
    {
        if (this.stats == null)
        {
            return;
        }

        float attackSpeed = this.stats.Get("attack_speed").AsSingle();
        float attackRange = this.stats.Get("attack_range").AsSingle();

        this.attack_timer.WaitTime = 1.0 / attackSpeed;
        this.range_preview.radius = attackRange;
        this.range_collision.SetDeferred("polygon", build_ellipse_polygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
    }

    public void _on_exp_data_change(Variant new_exp_data)
    {
        this.exp_data = new_exp_data.AsGodotObject();
    }

    private void _on_tower_selected(Variant tower_var)
    {
        this.current_tower_selected = tower_var.AsGodotObject() as Tower;
        this.range_preview.Visible = this.current_tower_selected == this;
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!this._enabled)
        {
            return;
        }

        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        GodotObject towersManager = GetTowersManager();
        towersManager?.Call("select_tower", this);
    }

    private void _on_mouse_entered()
    {
        GodotObject towersManager = GetTowersManager();
        towersManager?.EmitSignal("tower_hovered", this);
        this._show_range();
    }

    private void _on_mouse_exit()
    {
        GodotObject towersManager = GetTowersManager();
        towersManager?.EmitSignal("tower_unhovered", this);

        if (this.current_tower_selected != this)
        {
            this._hide_range();
        }
    }

    private void _show_range()
    {
        this.range_tween?.Kill();
        this.range_tween = CreateTween();
        this.range_preview.Visible = true;
        this.range_tween.TweenProperty(this.range_preview, "self_modulate:a", 1.0f, 0.1f).SetTrans(Tween.TransitionType.Sine);
    }

    private void _hide_range()
    {
        this.range_tween?.Kill();
        this.range_tween = CreateTween();
        this.range_tween.TweenProperty(this.range_preview, "self_modulate:a", 0.0f, 0.5f).SetTrans(Tween.TransitionType.Sine);
        this.range_tween.TweenCallback(Callable.From(() => this.range_preview.Visible = false));
    }

    protected GodotObject GetRunContext()
    {
        return GetNodeOrNull<Node>("/root/RunContext") as GodotObject;
    }

    protected GodotObject GetTowersManager()
    {
        return GetRunContext()?.Get("towers_manager").AsGodotObject();
    }
}
