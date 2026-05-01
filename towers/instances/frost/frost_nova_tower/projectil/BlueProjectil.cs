using Godot;

[GlobalClass]
public partial class BlueProjectil : Area2D
{
    private static readonly PackedScene BlueExplosionScene = GD.Load<PackedScene>("uid://bntnbljmfy1p4");

    [Export] public float ExpandSpeed = 300.0f;
    [Export] public float YScale = 0.5f;
    [Export] public float thickness = 20.0f;
    [Export] public Color color = new(0.302f, 0.173f, 1.0f, 1.0f);

    private float _radius;
    private CircleShape2D _shape;
    private Attack _attack;
    private float _maxAreaRange;
    private EnemyDebuff _frostDebuff;

    private CollisionShape2D _collisionShape;
    private AudioStreamPlayer2D _blueAttack;

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape");
        _blueAttack = GetNode<AudioStreamPlayer2D>("BlueAttack");

        // Keep the nova wave above gameplay sprites so the ring is always visible.
        ZAsRelative = false;
        ZIndex = 50;

        _shape = _collisionShape.Shape as CircleShape2D;
        if (_shape != null)
        {
            _shape.Radius = 0.0f;
        }

        _blueAttack.Play();
    }

    public override void _Process(double delta)
    {
        _radius += ExpandSpeed * (float)delta;
        if (_shape != null)
        {
            _shape.Radius = _radius;
        }

        QueueRedraw();

        if (_radius >= _maxAreaRange)
        {
            QueueFree();
        }
    }

    public void SetStats(Attack attack, float areaRange, EnemyDebuff frostDebuff)
    {
        _attack = attack;
        _maxAreaRange = areaRange;
        _frostDebuff = frostDebuff;
    }

    public override void _Draw()
    {
        if (_maxAreaRange <= 0.0f)
        {
            return;
        }

        float radiusProgress = Mathf.Clamp(_radius / _maxAreaRange, 0.0f, 1.0f);
        const float fadeStartThreshold = 0.8f;

        float alphaFade;
        if (radiusProgress < fadeStartThreshold)
        {
            alphaFade = 1.0f;
        }
        else
        {
            float fadeProgress = (radiusProgress - fadeStartThreshold) / (1.0f - fadeStartThreshold);
            alphaFade = Mathf.Pow(1.0f - fadeProgress, 2.0f);
        }

        Color waveColor = new(color.R, color.G, color.B, color.A * alphaFade);

        DrawSetTransform(Vector2.Zero, 0.0f, new Vector2(1.0f, YScale));
        DrawCircle(Vector2.Zero, _radius, new Color(waveColor.R, waveColor.G, waveColor.B, waveColor.A * 0.12f));
        DrawArc(Vector2.Zero, _radius, 0, Mathf.Tau, 64, waveColor, thickness);
        DrawSetTransform(Vector2.Zero, 0.0f, Vector2.One);
    }

    private async void OnBodyEntered(Node2D body)
    {
        Node2D enemy = body;
        if (!GodotObject.IsInstanceValid(enemy))
        {
            return;
        }

        CpuParticles2D explosion = BlueExplosionScene.Instantiate<CpuParticles2D>();

        Enemy enemyNode = enemy as Enemy;
        enemyNode?.ApplyDamage(_attack);
        enemyNode?.ApplyDebuff(_frostDebuff);

        AddChild(explosion);
        explosion.GlobalPosition = enemy.GlobalPosition;
        explosion.Emitting = true;
        await ToSignal(explosion, CpuParticles2D.SignalName.Finished);
        explosion.QueueFree();
    }
}
