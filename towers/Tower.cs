using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class Tower : Node2D
{
    public event Action<TowerBuff> BuffAdded;
    public event Action<TowerBuff> BuffRemoved;
    public event Action<Tower> StatsChanged;
    public event Action AttackFired;
    public event Action<Node2D> TargetChanged;

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

    [Export] public Type TowerType = Type.FIRE;

    public GodotObject Data;
    public int BuildPrice { get; set; } = 0;

    protected Node2D _currentTarget;
    protected bool _enabled = false;
    protected bool _firstShot = true;

    public Tower CurrentTowerSelected;
    public Tween RangeTween;

    private TowerStats _stats;
    public TowerStats Stats
    {
        get => _stats;
        set
        {
            _stats = value;
            StatsChanged?.Invoke(this);
        }
    }

    public Vector2I TilePos;
    public string CompositeTileKey = string.Empty;

    private int _targetingMode = (int)TargetingMode.FIRST_IN_PROGRESS;
    public int CurrentTargetingMode
    {
        get => _targetingMode;
        set
        {
            _targetingMode = value;
            if (_areaDetector != null)
            {
                _areaDetector.TargetingType = value;
            }
        }
    }

    public int Level = 1;

    private TowerExpData _expData;
    public TowerExpData ExpData
    {
        get => _expData;
        set
        {
            _expData = value;
            StatsChanged?.Invoke(this);
        }
    }

    public string Id = string.Empty;
    public string TypeId = string.Empty;

    public readonly List<TowerBuff> Buffs = new();

    public TowerLogic TowerLogic;

    public Source DamageSource
    {
        get
        {
            return new Source(Source.SourceType.TOWER, TypeId, this);
        }
    }

    protected AreaDetector _areaDetector;
    protected RangePreview _rangePreview;
    protected CollisionPolygon2D _rangeCollision;
    protected Control _mouseDetector;
    protected Timer _attackTimer;
    protected CristalLight _cristalLight;
    protected Sprite2D _sprite2D;
    protected TowerStatsHandler _towerStatsHandler;
    protected Area2D _towerArea;
    protected CollisionPolygon2D _towerAreaCollision;

    private bool _eventsConnected;
    private TowersManager _towersManager;

    public override void _Ready()
    {
        _areaDetector = GetNode<AreaDetector>("AreaDetector");
        _rangePreview = GetNode<RangePreview>("RangePreview");
        _rangeCollision = GetNode<CollisionPolygon2D>("AreaDetector/RangeCollision");
        _mouseDetector = GetNode<Control>("MouseDetector");
        _attackTimer = GetNode<Timer>("AttackTimer");
        _cristalLight = GetNodeOrNull<CristalLight>("CristalLight");
        _sprite2D = GetNodeOrNull<Sprite2D>("Sprite2D");
        _towerStatsHandler = GetNode<TowerStatsHandler>("TowerStatsHandler");
        _towerArea = GetNode<Area2D>("TowerArea");
        _towerAreaCollision = GetNode<CollisionPolygon2D>("TowerArea/CollisionShape2D");

        TypeId = GetType().Name;

        (Data as TowerData)?.Build();
        TowerLogic = new TowerLogic(this);
        _towerAreaCollision.Polygon = BuildEllipsePolygon(TOWER_AREA_RADIUS, TOWER_AREA_RADIUS * ELLIPSE_Y_RATIO);

        _towerStatsHandler.StatsChanged += OnStatsChange;
        _towerStatsHandler.BuffApplied += AddBuff;
        _towerStatsHandler.BuffExpired += RemoveBuff;
        _towerStatsHandler.SetData(Data as TowerData, (int)TowerType);

        _areaDetector.TargetChanged += OnTargetChange;
        CurrentTargetingMode = _targetingMode;
    }

    public override void _ExitTree()
    {
        if (_areaDetector != null)
        {
            _areaDetector.TargetChanged -= OnTargetChange;
        }

        if (_towerStatsHandler != null)
        {
            _towerStatsHandler.StatsChanged -= OnStatsChange;
            _towerStatsHandler.BuffApplied -= AddBuff;
            _towerStatsHandler.BuffExpired -= RemoveBuff;
        }

        if (_eventsConnected)
        {
            if (_towersManager != null)
            {
                _towersManager.TowerSelected -= OnTowerSelected;
                _towersManager = null;
            }

            _mouseDetector.GuiInput -= OnGuiInput;
            _mouseDetector.MouseEntered -= OnMouseEntered;
            _mouseDetector.MouseExited -= OnMouseExit;
            _eventsConnected = false;
        }
    }

    public static Vector2[] BuildEllipsePolygon(float radius_x, float radius_y, int segments = 48)
    {
        Vector2[] points = new Vector2[segments];
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Tau * i / (float)segments;
            points[i] = new Vector2(Mathf.Cos(angle) * radius_x, Mathf.Sin(angle) * radius_y);
        }

        return points;
    }

    public static string TargetingModeToString(int mode)
    {
        return mode switch
        {
            (int)TargetingMode.FIRST_IN_PROGRESS => "Progress",
            (int)TargetingMode.HIGH_HP => "High Health",
            (int)TargetingMode.LOW_HP => "Low Health",
            _ => "Unknown",
        };
    }

    public void PhantomMode()
    {
        if (_sprite2D != null)
        {
            _sprite2D.Modulate = PHANTOM_COLOR;
        }

        if (_rangePreview != null)
        {
            _rangePreview.Visible = false;
        }
    }

    public void NormalColor()
    {
        if (_sprite2D != null)
        {
            _sprite2D.Modulate = Colors.White;
        }

        ShowRange();
    }

    public virtual void PlacementMode()
    {
        _enabled = false;
        _areaDetector.Monitoring = false;
        _towerArea.Monitorable = false;
    }

    public virtual void Enable()
    {
        AsyncTaskHelper.FireAndForget(EnableAsync(), "Tower.EnableAsync");
    }

    private async Task EnableAsync()
    {
        if (_sprite2D != null)
        {
            _sprite2D.Modulate = Colors.White;
        }

        _enabled = true;
        _areaDetector.Monitoring = true;
        _towerArea.Monitorable = true;

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        if (_eventsConnected)
        {
            return;
        }

        _towersManager = GetTowersManager();
        if (_towersManager != null)
        {
            _towersManager.TowerSelected += OnTowerSelected;
        }

        _mouseDetector.GuiInput += OnGuiInput;
        _mouseDetector.MouseEntered += OnMouseEntered;
        _mouseDetector.MouseExited += OnMouseExit;
        _eventsConnected = true;
    }

    public virtual void AddBuff(TowerBuff towerBuff)
    {
        if (towerBuff == null)
        {
            return;
        }

        Buffs.Add(towerBuff);
        if (towerBuff is TowerBuffStatsModifier)
        {
            _towerStatsHandler.AddBuff(towerBuff);
        }

        BuffAdded?.Invoke(towerBuff);
    }

    public virtual void RemoveBuff(string sourceId)
    {
        List<TowerBuff> removedBuffs = new();

        for (int i = Buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buff = Buffs[i];
            if (buff == null)
            {
                continue;
            }

            string buffSourceId = buff.Source?.TypeId ?? string.Empty;
            if (buffSourceId != sourceId)
            {
                continue;
            }

            removedBuffs.Add(buff);
            Buffs.RemoveAt(i);
        }

        _towerStatsHandler.RemoveBuff(sourceId);

        foreach (TowerBuff removedBuff in removedBuffs)
        {
            BuffRemoved?.Invoke(removedBuff);
        }
    }

    public virtual void Upgrade()
    {
        RunContext runContext = RunContext.Instance;
        Economy economy = runContext?.Economy;
        TowerData towerData = Data as TowerData;
        if (economy != null && towerData != null)
        {
            int currentGold = economy.Gold;
            int upgradePrice = towerData.UpgradePrice;
            economy.Gold = currentGold - upgradePrice;
        }

        Level += 1;
        _towerStatsHandler.LevelUp(Level);
    }

    public bool IsMaxLevel()
    {
        return Level >= MAX_LEVEL;
    }

    public void CopyTowerData(Tower fromTower)
    {
        if (fromTower == null)
        {
            return;
        }

        _areaDetector.TargetsInRange.Clear();
        foreach (Node2D enemy in fromTower._areaDetector.TargetsInRange)
        {
            _areaDetector.TargetsInRange.Add(enemy);
        }

        _areaDetector.CurrentTarget = fromTower._areaDetector.CurrentTarget;
        CurrentTargetingMode = fromTower.CurrentTargetingMode;
    }

    public Attack GetAttack()
    {
        bool isCritical = IsCriticalHit();

        float baseDamage = Stats?.Damage ?? 0f;
        float critDamage = Stats?.CritDamage ?? 0f;
        float attackDamage = isCritical ? baseDamage * (1f + critDamage / 100f) : baseDamage;

        Attack attack = new(attackDamage, DamageSource, isCritical);
        Enemy targetEnemy = _currentTarget as Enemy;
        AttackContext ctx = new(targetEnemy, BuildAttackModel(attack), BuildTowerModel());
        Hooks.OnBeforeAttack(Hooks.GetListenersFromRuntime(), ctx);
        attack.Damage = ctx.RebuildAttack();

        return attack;
    }

    private AttackModel BuildAttackModel(Attack attack)
    {
        var model = new AttackModel
        {
            Damage = attack?.Damage ?? 0f,
            CritChance = attack?.CritChance ?? 0f,
            IsCritical = attack?.IsCritical ?? false,
            IsExecution = attack?.IsExecution ?? false,
            Hits = attack?.Hits ?? 1,
            Bounces = attack?.Bounces ?? 0,
        };

        Source source = attack?.Source;
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

    private TowerModel BuildTowerModel()
    {
        TowerModel.TowerType towerType = TowerType switch
        {
            Type.FIRE => TowerModel.TowerType.Fire,
            Type.LIGHTNING => TowerModel.TowerType.Lightning,
            Type.FROST => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        return new TowerModel
        {
            Id = Id,
            TypeId = TypeId,
            Type = towerType,
        };
    }

    public bool IsCriticalHit()
    {
        float randomValue = GD.Randf();
        float critChance = Stats?.CritChance ?? 0f;
        return randomValue < (critChance / 100f);
    }

    private void OnTargetChange(Node2D enemy)
    {
        _currentTarget = enemy;
        TargetChanged?.Invoke(enemy);

        if (_firstShot && GodotObject.IsInstanceValid(_currentTarget))
        {
            Fire();
            AttackFired?.Invoke();
            _attackTimer.Start();
            _firstShot = false;
        }
    }

    private void OnAttackTimerTimeout()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || !_enabled)
        {
            _firstShot = true;
            return;
        }

        Fire();
        AttackFired?.Invoke();
    }

    protected virtual void Fire() { }

    public virtual void OnStatsChange(TowerStats newStats)
    {
        Stats = newStats;
        ApplyStatsChanges();
    }

    public virtual void ApplyStatsChanges()
    {
        if (Stats == null)
        {
            return;
        }

        float attackSpeed = Stats.AttackSpeed;
        float attackRange = Stats.AttackRange;

        _attackTimer.WaitTime = 1.0 / attackSpeed;
        _rangePreview.radius = attackRange;
        _rangeCollision.SetDeferred("polygon", BuildEllipsePolygon(attackRange, attackRange * ELLIPSE_Y_RATIO));
    }

    public void OnExpDataChange(Variant newExpData)
    {
        ExpData = newExpData.Obj as TowerExpData;
    }

    private void OnTowerSelected(Tower tower)
    {
        CurrentTowerSelected = tower;
        _rangePreview.Visible = CurrentTowerSelected == this;
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

        _towersManager?.SelectTower(this);
    }

    private void OnMouseEntered()
    {
        _towersManager?.EmitTowerHovered(this);
        ShowRange();
    }

    private void OnMouseExit()
    {
        _towersManager?.EmitTowerUnhovered(this);

        if (CurrentTowerSelected != this)
        {
            HideRange();
        }
    }

    private void ShowRange()
    {
        RangeTween?.Kill();
        RangeTween = CreateTween();
        _rangePreview.Visible = true;
        RangeTween.TweenProperty(_rangePreview, "self_modulate:a", 1.0f, 0.1f).SetTrans(Tween.TransitionType.Sine);
    }

    private void HideRange()
    {
        RangeTween?.Kill();
        RangeTween = CreateTween();
        RangeTween.TweenProperty(_rangePreview, "self_modulate:a", 0.0f, 0.5f).SetTrans(Tween.TransitionType.Sine);
        RangeTween.Finished += OnHideRangeTweenFinished;
    }

    private void OnHideRangeTweenFinished()
    {
        if (_rangePreview != null)
        {
            _rangePreview.Visible = false;
        }
    }

    protected RunContext GetRunContext()
    {
        return RunContext.Instance;
    }

    protected TowersManager GetTowersManager()
    {
        RunContext runContext = RunContext.Instance;
        return runContext?.TowersManager;
    }
}

