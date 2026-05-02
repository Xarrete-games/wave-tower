using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{
    public event Action<Enemy, Attack> Died;
    public event Action<Enemy> TargetReached;

    private static readonly PackedScene GOLD_DROPPED = GD.Load<PackedScene>("uid://cxs4ar5enx4mn");
    private static readonly PackedScene DAMAGE_NUMBERS = GD.Load<PackedScene>("uid://bkiu4qgh3ug1m");
    private const float WAYPOINT_ARRIVAL_THRESHOLD = 8.0f;
    private const float STEERING_FACTOR = 8.0f;

    [Export] public float BaseSpeed = 80.0f;
    [Export] public float MaxHealth = 50.0f;
    [Export] public int GoldValue = 1;
    [Export] public int Damage = 1;

    public float Health;
    public HealthBar HealthBar;
    public DebuffHandler DebuffHandler;

    private Tween _hitTween;
    private bool _lastIsRightDirection;
    private Color _defaultModulateColor = Colors.White;
    private float _baseSpeed = 100.0f;
    private float _speedMultiplier = 1.0f;
    private bool _isRightDirection = true;
    private bool _isDead;

    private readonly List<Vector2> _waypoints = new();
    private int _currentWaypointIndex;
    private float _totalPathLength;
    private Vector2 _velocity = Vector2.Zero;

    private AnimationPlayer _animationPlayer;
    private AnimatedSprite2D _animatedSprite2D;
    private CollisionShape2D _collisionShape2D;
    private Marker2D _targetPositionLeft;
    private Marker2D _targetPositionRight;

    public bool IsEnabled { get; private set; } = true;

    public float Speed
    {
        get => _baseSpeed * _speedMultiplier;
        set => _baseSpeed = value;
    }

    public float SpeedMultiplier
    {
        get => _speedMultiplier;
        set => _speedMultiplier = value;
    }

    public Vector2 TargetPosition => _isRightDirection ? _targetPositionRight.GlobalPosition : _targetPositionLeft.GlobalPosition;

    public Vector2 InversedTargetPosition => _isRightDirection ? _targetPositionLeft.GlobalPosition : _targetPositionRight.GlobalPosition;

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "Enemy.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        HealthBar = GetNode<HealthBar>("HealthBar");
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
        DebuffHandler = GetNode<DebuffHandler>("DebuffHandler");
        _targetPositionLeft = GetNode<Marker2D>("TargetPositionLeft");
        _targetPositionRight = GetNode<Marker2D>("TargetPositionRight");

        Speed = BaseSpeed;
        HealthBar.SetMaxHealth(MaxHealth);
        SetHealth(MaxHealth);

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);
        IsEnabled = true;
    }

    public override void _Process(double delta)
    {
        DebuffHandler.UpdateAll(this);

        if (_waypoints.Count > 0)
        {
            ProcessWaypoints((float)delta);
        }
    }

    private void ProcessWaypoints(float delta)
    {
        if (_currentWaypointIndex >= _waypoints.Count)
        {
            OnTargetReached();
            return;
        }

        Vector2 targetPoint = _waypoints[_currentWaypointIndex];
        float distance = GlobalPosition.DistanceTo(targetPoint);

        if (distance <= WAYPOINT_ARRIVAL_THRESHOLD)
        {
            _currentWaypointIndex += 1;
            return;
        }

        float previousGlobalX = GlobalPosition.X;
        float previousGlobalY = GlobalPosition.Y;

        Vector2 direction = (targetPoint - GlobalPosition).Normalized();
        Vector2 desiredVelocity = direction * Speed;
        _velocity = _velocity.MoveToward(desiredVelocity, STEERING_FACTOR * Speed * delta);

        Vector2 displacement = _velocity * delta;
        if (displacement.Length() >= distance)
        {
            GlobalPosition = targetPoint;
        }
        else
        {
            GlobalPosition += displacement;
        }

        UpdateSpriteDirection(previousGlobalX);
        UpdateAnimation(previousGlobalY);
    }

    private void UpdateSpriteDirection(float previousX)
    {
        _isRightDirection = GlobalPosition.X > previousX;
        Marker2D marker = _isRightDirection ? _targetPositionRight : _targetPositionLeft;

        Vector2 hbPos = HealthBar.Position;
        hbPos.X = marker.Position.X - HealthBar.Size.X * HealthBar.Scale.X * 0.5f;
        HealthBar.Position = hbPos;

        if (_isRightDirection != _lastIsRightDirection)
        {
            _animatedSprite2D.FlipH = !_animatedSprite2D.FlipH;
            _lastIsRightDirection = _isRightDirection;
        }
    }

    private void UpdateAnimation(float previousY)
    {
        string animation = previousY > GlobalPosition.Y ? "top right" : "down right";
        if (_animatedSprite2D.Animation != animation || !_animatedSprite2D.IsPlaying())
        {
            _animatedSprite2D.Play(animation);
        }
    }

    public void SetWaypoints(IReadOnlyList<Vector2> waypoints)
    {
        _waypoints.Clear();
        for (int i = 0; i < waypoints.Count; i++)
        {
            _waypoints.Add(waypoints[i]);
        }

        _currentWaypointIndex = 0;
        _velocity = Vector2.Zero;
        _totalPathLength = ComputePathLength(_waypoints);
    }

    private float ComputePathLength(IReadOnlyList<Vector2> points)
    {
        float length = 0.0f;
        for (int i = 1; i < points.Count; i++)
        {
            length += points[i - 1].DistanceTo(points[i]);
        }

        return length;
    }

    public float GetProgressRatio()
    {
        if (_waypoints.Count == 0 || _totalPathLength <= 0.0f)
        {
            return 0.0f;
        }

        if (_currentWaypointIndex >= _waypoints.Count)
        {
            return 1.0f;
        }

        float covered = 0.0f;
        for (int i = 1; i < _currentWaypointIndex; i++)
        {
            covered += _waypoints[i - 1].DistanceTo(_waypoints[i]);
        }

        Vector2 segStart = _currentWaypointIndex > 0 ? _waypoints[_currentWaypointIndex - 1] : _waypoints[0];
        covered += segStart.DistanceTo(GlobalPosition);
        return Mathf.Clamp(covered / _totalPathLength, 0.0f, 1.0f);
    }

    public bool HasWaypoints()
    {
        return _waypoints.Count > 0 && _currentWaypointIndex < _waypoints.Count;
    }

    public Vector2 GetCurrentWaypoint()
    {
        if (_currentWaypointIndex < _waypoints.Count)
        {
            return _waypoints[_currentWaypointIndex];
        }

        return Vector2.Zero;
    }

    public void Disable()
    {
        IsEnabled = false;
        if (_animatedSprite2D == null || _collisionShape2D == null || HealthBar == null)
        {
            return;
        }

        _animatedSprite2D.Visible = false;
        _collisionShape2D.Disabled = true;
        HealthBar.Visible = false;
    }

    public void Enable()
    {
        AsyncTaskHelper.FireAndForget(EnableAsync(), "Enemy.EnableAsync");
    }

    private async Task EnableAsync()
    {
        IsEnabled = true;
        if (_animatedSprite2D == null || _collisionShape2D == null || HealthBar == null)
        {
            return;
        }

        _animatedSprite2D.Visible = true;
        HealthBar.Visible = true;

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _collisionShape2D.Disabled = false;
    }

    public float GetPercentageRemainingHealth()
    {
        if (MaxHealth <= 0.0f)
        {
            return 0.0f;
        }

        float healthRatio = Health / MaxHealth;
        float percentage = healthRatio * 100.0f;
        return Mathf.Min(100.0f, percentage);
    }

    public float GetRemainingHealth()
    {
        return Health;
    }

    public int GetDebuffStacks(int debuffType)
    {
        return DebuffHandler.GetStacks(debuffType);
    }

    public List<EnemyDebuff> GetActiveDebuffs()
    {
        return DebuffHandler.GetActiveDebuffs();
    }

    public bool HasAnyDebuff()
    {
        return DebuffHandler.HasAnyDebuff();
    }

    public void ApplyDebuff(EnemyDebuff debuff, int amount = 1)
    {
        DebuffHandler.AddDebuff(debuff, amount, this);
    }

    public void ApplyDamage(Attack attack)
    {
        if (_isDead)
        {
            return;
        }

        DamageContext ctx = new(BuildAttackModel(attack), this);
        Hooks.OnBeforeDamage(Hooks.GetListenersFromRuntime(), ctx);
        float modifiedDamage = ctx.GetTotalDamage();

        attack.Damage = modifiedDamage;

        SetHealth(Health - attack.Damage);
        PlayHitAnimation();
        ShowDamage(attack);

        if (Health <= 0.0f && !_isDead)
        {
            _isDead = true;
            Die(attack);
        }
    }

    private void PlayHitAnimation()
    {
        AsyncTaskHelper.FireAndForget(PlayHitAnimationAsync(), "Enemy.PlayHitAnimationAsync");
    }

    private async Task PlayHitAnimationAsync()
    {
        if (_hitTween != null && _hitTween.IsRunning())
        {
            _hitTween.Kill();
        }

        _hitTween = CreateTween();
        _hitTween.TweenProperty(_animatedSprite2D, "modulate", Colors.Red, 0.2f);
        await ToSignal(_hitTween, Tween.SignalName.Finished);
        _animatedSprite2D.Modulate = _defaultModulateColor;
    }

    private void Die(Attack attack)
    {
        Died?.Invoke(this, attack);
        Hooks.OnEnemyDie(Hooks.GetListenersFromRuntime(), this, BuildAttackModel(attack));
        ShowGoldDropped();

        RunContext runContext = RunContext.Instance;
        if (runContext?.Economy != null)
        {
            runContext.Economy.Gold += GoldValue;
        }

        QueueFree();
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

    private void ShowDamage(Attack attack)
    {
        const int MAX_OFFSET = 30;
        DamageNumbers damageNumbers = DAMAGE_NUMBERS.Instantiate<DamageNumbers>();
        Vector2 basePosition = TargetPosition;

        int randomOffsetX = (int)GD.RandRange(-MAX_OFFSET, MAX_OFFSET);
        int randomOffsetY = (int)GD.RandRange(-MAX_OFFSET, MAX_OFFSET);

        damageNumbers.GlobalPosition = basePosition + new Vector2(randomOffsetX, randomOffsetY);

        GetTree().Root.AddChild(damageNumbers);
        damageNumbers.SetAttack(attack);
    }

    private void ShowGoldDropped()
    {
        GoldDropped goldDropped = GOLD_DROPPED.Instantiate<GoldDropped>();
        GetTree().Root.AddChild(goldDropped);
        goldDropped.SetGold(GoldValue);
        goldDropped.GlobalPosition = TargetPosition;
    }

    private void OnTargetReached()
    {
        TargetReached?.Invoke(this);
        QueueFree();
    }

    private void SetHealth(float newValue)
    {
        Health = newValue;
        HealthBar.UpdateHealth(Health);
    }
}

