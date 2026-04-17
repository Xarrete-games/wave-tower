using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Tower : Node2D
{
    public event Action<TowerBuff> buff_added;
    public event Action<TowerBuff> buff_removed;
    public event Action<Tower> stats_changed;
    public event Action attack_fired;
    public event Action<Node2D> on_target_change;

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

    private TowerStats _stats;
    public TowerStats stats
    {
        get => _stats;
        set
        {
            _stats = value;
            stats_changed?.Invoke(this);
        }
    }

    public Vector2I tile_pos;
    public string composite_tile_key = string.Empty;

    private int _targeting_mode = (int)TargetingMode.FIRST_IN_PROGRESS;
    public int targeting_mode
    {
        get => _targeting_mode;
        set
        {
            _targeting_mode = value;
            if (area_detector != null)
            {
                area_detector.targeting_type = value;
            }
        }
    }

    public int level = 1;

    private TowerExpData _exp_data;
    public TowerExpData exp_data
    {
        get => _exp_data;
        set
        {
            _exp_data = value;
            stats_changed?.Invoke(this);
        }
    }

    public string id = string.Empty;
    public string type_id = string.Empty;

    public readonly List<TowerBuff> buffs = new();

    public TowerLogic tower_logic;

    public Source damage_source
    {
        get
        {
            return new Source(Source.SourceType.TOWER, type_id, this);
        }
    }

    protected AreaDetector area_detector;
    protected RangePreview range_preview;
    protected CollisionPolygon2D range_collision;
    protected Control mouse_detector;
    protected Timer attack_timer;
    protected CristalLight cristal_light;
    protected Sprite2D sprite_2d;
    protected TowerStatsHandler tower_stats_handler;
    protected Area2D tower_area;
    protected CollisionPolygon2D tower_area_collision;

    private bool _eventsConnected;
    private TowersManager _towersManager;

    public override void _Ready()
    {
        area_detector = GetNode<AreaDetector>("AreaDetector");
        range_preview = GetNode<RangePreview>("RangePreview");
        range_collision = GetNode<CollisionPolygon2D>("AreaDetector/RangeCollision");
        mouse_detector = GetNode<Control>("MouseDetector");
        attack_timer = GetNode<Timer>("AttackTimer");
        cristal_light = GetNodeOrNull<CristalLight>("CristalLight");
        sprite_2d = GetNodeOrNull<Sprite2D>("Sprite2D");
        tower_stats_handler = GetNode<TowerStatsHandler>("TowerStatsHandler");
        tower_area = GetNode<Area2D>("TowerArea");
        tower_area_collision = GetNode<CollisionPolygon2D>("TowerArea/CollisionShape2D");

        type_id = GetType().Name;

        (data as TowerData)?.build();
        tower_logic = new TowerLogic(this);
        tower_area_collision.Polygon = build_ellipse_polygon(TOWER_AREA_RADIUS, TOWER_AREA_RADIUS * ELLIPSE_Y_RATIO);

        tower_stats_handler.stats_change += _on_stats_change;
        tower_stats_handler.buff_applied += add_buff;
        tower_stats_handler.buff_expired += remove_buff;
        tower_stats_handler.set_data(data as TowerData, (int)type);

        area_detector.target_change += OnTargetChange;
        targeting_mode = _targeting_mode;
    }

    public override void _ExitTree()
    {
        if (area_detector != null)
        {
            area_detector.target_change -= OnTargetChange;
        }

        if (tower_stats_handler != null)
        {
            tower_stats_handler.stats_change -= _on_stats_change;
            tower_stats_handler.buff_applied -= add_buff;
            tower_stats_handler.buff_expired -= remove_buff;
        }

        if (_eventsConnected)
        {
            if (_towersManager != null)
            {
                _towersManager.tower_selected -= OnTowerSelected;
                _towersManager = null;
            }

            mouse_detector.GuiInput -= OnGuiInput;
            mouse_detector.MouseEntered -= OnMouseEntered;
            mouse_detector.MouseExited -= OnMouseExit;
            _eventsConnected = false;
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
        if (sprite_2d != null)
        {
            sprite_2d.Modulate = PHANTOM_COLOR;
        }

        if (range_preview != null)
        {
            range_preview.Visible = false;
        }
    }

    public void normal_color()
    {
        if (sprite_2d != null)
        {
            sprite_2d.Modulate = Colors.White;
        }

        ShowRange();
    }

    public virtual void placement_mode()
    {
        _enabled = false;
        area_detector.Monitoring = false;
        tower_area.Monitorable = false;
    }

    public virtual async void enable()
    {
        if (sprite_2d != null)
        {
            sprite_2d.Modulate = Colors.White;
        }

        _enabled = true;
        area_detector.Monitoring = true;
        tower_area.Monitorable = true;

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        if (_eventsConnected)
        {
            return;
        }

        _towersManager = GetTowersManager();
        if (_towersManager != null)
        {
            _towersManager.tower_selected += OnTowerSelected;
        }

        mouse_detector.GuiInput += OnGuiInput;
        mouse_detector.MouseEntered += OnMouseEntered;
        mouse_detector.MouseExited += OnMouseExit;
        _eventsConnected = true;
    }

    public virtual void add_buff(TowerBuff tower_buff)
    {
        if (tower_buff == null)
        {
            return;
        }

        buffs.Add(tower_buff);
        if (tower_buff is TowerBuffStatsModifier)
        {
            tower_stats_handler.add_buff(tower_buff);
        }

        buff_added?.Invoke(tower_buff);
    }

    public virtual void remove_buff(string source_id)
    {
        List<TowerBuff> removedBuffs = new();

        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buff = buffs[i];
            if (buff == null)
            {
                continue;
            }

            string buffSourceId = buff.source?.TypeId ?? string.Empty;
            if (buffSourceId != source_id)
            {
                continue;
            }

            removedBuffs.Add(buff);
            buffs.RemoveAt(i);
        }

        tower_stats_handler.remove_buff(source_id);

        foreach (TowerBuff removedBuff in removedBuffs)
        {
            buff_removed?.Invoke(removedBuff);
        }
    }

    public virtual void upgrade()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        Economy economy = runContext?.economy;
        TowerData towerData = data as TowerData;
        if (economy != null && towerData != null)
        {
            int currentGold = economy.gold;
            int upgradePrice = towerData.upgrade_price;
            economy.gold = currentGold - upgradePrice;
        }

        level += 1;
        tower_stats_handler.level_up(level);
    }

    public bool is_max_level()
    {
        return level >= MAX_LEVEL;
    }

    public void copy_tower_data(Tower from_tower)
    {
        if (from_tower == null)
        {
            return;
        }

        area_detector.targets_in_range.Clear();
        foreach (Node2D enemy in from_tower.area_detector.targets_in_range)
        {
            area_detector.targets_in_range.Add(enemy);
        }

        area_detector.current_target = from_tower.area_detector.current_target;
        targeting_mode = from_tower.targeting_mode;
    }

    public Attack _get_attack()
    {
        bool is_critical = _is_critical_hit();

        float baseDamage = stats?.damage ?? 0f;
        float critDamage = stats?.critic_damage ?? 0f;
        float attackDamage = is_critical ? baseDamage * (1f + critDamage / 100f) : baseDamage;

        Attack attack = new(attackDamage, damage_source, is_critical);
        Enemy targetEnemy = _current_target as Enemy;
        AttackContext ctx = new(BuildEnemyModel(targetEnemy), BuildAttackModel(attack), BuildTowerModel());
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
        TowerModel.TowerType towerType = type switch
        {
            Type.FIRE => TowerModel.TowerType.Fire,
            Type.LIGHTNING => TowerModel.TowerType.Lightning,
            Type.FROST => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        return new TowerModel
        {
            Id = id,
            TypeId = type_id,
            Type = towerType,
        };
    }

    public bool _is_critical_hit()
    {
        float randomValue = GD.Randf();
        float critChance = stats?.critic_chance ?? 0f;
        return randomValue < (critChance / 100f);
    }

    private void OnTargetChange(Node2D enemy)
    {
        _current_target = enemy;
        on_target_change?.Invoke(enemy);

        if (_first_shot && GodotObject.IsInstanceValid(_current_target))
        {
            _fire();
            attack_fired?.Invoke();
            attack_timer.Start();
            _first_shot = false;
        }
    }

    private void OnAttackTimerTimeout()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || !_enabled)
        {
            _first_shot = true;
            return;
        }

        _fire();
        attack_fired?.Invoke();
    }

    protected virtual void _fire() { }

    public virtual void _on_stats_change(TowerStats new_stats)
    {
        stats = new_stats;
        _apply_stats_changes();
    }

    public virtual void _apply_stats_changes()
    {
        if (stats == null)
        {
            return;
        }

        float attackSpeed = stats.attack_speed;
        float attackRange = stats.attack_range;

        attack_timer.WaitTime = 1.0 / attackSpeed;
        range_preview.radius = attackRange;
        range_collision.SetDeferred("polygon", build_ellipse_polygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
    }

    public void _on_exp_data_change(Variant new_exp_data)
    {
        exp_data = new_exp_data.Obj as TowerExpData;
    }

    private void OnTowerSelected(Tower tower)
    {
        current_tower_selected = tower;
        range_preview.Visible = current_tower_selected == this;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!_enabled)
        {
            return;
        }

        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        _towersManager?.select_tower(this);
    }

    private void OnMouseEntered()
    {
        _towersManager?.emit_tower_hovered(this);
        ShowRange();
    }

    private void OnMouseExit()
    {
        _towersManager?.emit_tower_unhovered(this);

        if (current_tower_selected != this)
        {
            HideRange();
        }
    }

    private void ShowRange()
    {
        range_tween?.Kill();
        range_tween = CreateTween();
        range_preview.Visible = true;
        range_tween.TweenProperty(range_preview, "self_modulate:a", 1.0f, 0.1f).SetTrans(Tween.TransitionType.Sine);
    }

    private void HideRange()
    {
        range_tween?.Kill();
        range_tween = CreateTween();
        range_tween.TweenProperty(range_preview, "self_modulate:a", 0.0f, 0.5f).SetTrans(Tween.TransitionType.Sine);
        range_tween.Finished += OnHideRangeTweenFinished;
    }

    private void OnHideRangeTweenFinished()
    {
        if (range_preview != null)
        {
            range_preview.Visible = false;
        }
    }

    protected GodotObject GetRunContext()
    {
        return GetNodeOrNull<Node>("/root/RunContext") as GodotObject;
    }

    protected TowersManager GetTowersManager()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        return runContext?.towers_manager;
    }
}
