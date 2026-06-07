using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class TowerNode : Node2D
{
    public event Action<TowerBuff> BuffAdded;
    public event Action<TowerBuff> BuffRemoved;
    public event Action<TowerNode> StatsChanged;
    public event Action AttackFired;
    public event Action<Node2D> TargetChanged;

    public static readonly Color PHANTOM_COLOR = new(1f, 1f, 1f, 0.5f);

    public const float ELLIPSE_Y_RATIO = 0.5f;
    public const float TOWER_AREA_RADIUS = 47.042534f;

    [Export] public Tower.Type TowerType = Tower.Type.FIRE;

    public Tower Tower { get; private set; } = new();

    private TowerData _data;

    public TowerData Data
    {
        get => _data;
        set
        {
            _data = value;
            Tower.UpgradePrice = value?.UpgradePrice ?? 0;
        }
    }

    public int BuildPrice
    {
        get => Tower.BuildPrice;
        set => Tower.BuildPrice = value;
    }

    protected Node2D _currentTarget;
    protected bool _enabled = false;
    protected bool _firstShot = true;

    public TowerNode CurrentTowerSelected;
    public Tween RangeTween;

    public TowerStats Stats
    {
        get => Tower.Stats;
        set
        {
            Tower.Stats = value;
            StatsChanged?.Invoke(this);
        }
    }

    public Vector2I TilePos;
    public string CompositeTileKey = string.Empty;

    private int _targetingMode = (int)Tower.TargetingMode.FIRST_IN_PROGRESS;
    public int CurrentTargetingMode
    {
        get => _targetingMode;
        set
        {
            _targetingMode = value;
            Tower.CurrentTargetingMode = value;
            if (_areaDetector != null)
            {
                _areaDetector.TargetingType = value;
            }
        }
    }

    public int Level
    {
        get => Tower.Level;
        set => Tower.Level = value;
    }

    public TowerExpData ExpData
    {
        get => Tower.ExpData;
        set
        {
            Tower.ExpData = value;
            StatsChanged?.Invoke(this);
        }
    }

    public string Id
    {
        get => Tower.Id;
        set => Tower.Id = value;
    }

    public string TypeId
    {
        get => Tower.TypeId;
        set => Tower.TypeId = value;
    }

    public List<TowerBuff> Buffs => Tower.Buffs;

    public Source DamageSource
    {
        get
        {
            return Tower.DamageSource;
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
        Tower.TowerType = TowerType;

        Data?.Build();
        _towerAreaCollision.Polygon = BuildEllipsePolygon(TOWER_AREA_RADIUS, TOWER_AREA_RADIUS * ELLIPSE_Y_RATIO);

        _towerStatsHandler.StatsChanged += OnStatsChange;
        _towerStatsHandler.BuffApplied += AddBuff;
        _towerStatsHandler.BuffExpired += RemoveBuff;
        _towerStatsHandler.SetData(Data, (int)TowerType);

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
        AsyncTaskHelper.FireAndForget(EnableAsync(), "TowerNode.EnableAsync");
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

        Tower.AddBuff(towerBuff);
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
            Tower.RemoveBuffAt(i);
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
        Economy economy = runContext.Economy;
        Tower.Upgrade(economy);
        _towerStatsHandler.LevelUp(Level);
    }

    public bool IsMaxLevel()
    {
        return Tower.IsMaxLevel();
    }

    public void CopyTowerData(TowerNode fromTower)
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
        AttackContext ctx = new(targetEnemy, Tower.BuildAttackModel(attack), Tower.BuildTowerModel());
        Hooks.OnBeforeAttack(Hooks.GetListenersFromRuntime(), ctx);
        attack.Damage = ctx.RebuildAttack();

        return attack;
    }

    public bool IsCriticalHit()
    {
        float randomValue = GD.Randf();
        return Tower.IsCriticalHit(randomValue);
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

    private void OnTowerSelected(TowerNode tower)
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
        return runContext.TowersManager;
    }
}
