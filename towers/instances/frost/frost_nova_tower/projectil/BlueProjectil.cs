using Godot;

[GlobalClass]
public partial class BlueProjectil : Area2D
{
    private static readonly PackedScene BLUE_EXPLOSION = GD.Load<PackedScene>("uid://bntnbljmfy1p4");

    [Export] public float expand_speed = 300.0f;
    [Export] public float y_scale = 0.5f;
    [Export] public float thickness = 20.0f;
    [Export] public Color color = new(0.302f, 0.173f, 1.0f, 1.0f);

    private float _radius;
    private CircleShape2D _shape;
    private Attack _attack;
    private float _max_area_range;
    private GodotObject _frost_debuff;

    private CollisionShape2D _collision_shape;
    private AudioStreamPlayer2D _blue_attack;

    public override void _Ready()
    {
        this._collision_shape = GetNode<CollisionShape2D>("CollisionShape");
        this._blue_attack = GetNode<AudioStreamPlayer2D>("BlueAttack");

        // Keep the nova wave above gameplay sprites so the ring is always visible.
        this.ZAsRelative = false;
        this.ZIndex = 50;

        this._shape = this._collision_shape.Shape as CircleShape2D;
        if (this._shape != null)
        {
            this._shape.Radius = 0.0f;
        }

        this._blue_attack.Play();
    }

    public override void _Process(double delta)
    {
        this._radius += this.expand_speed * (float)delta;
        if (this._shape != null)
        {
            this._shape.Radius = this._radius;
        }

        QueueRedraw();

        if (this._radius >= this._max_area_range)
        {
            QueueFree();
        }
    }

    public void set_stats(Attack attack, float area_range, Variant frost_debuff)
    {
        this._attack = attack;
        this._max_area_range = area_range;
        this._frost_debuff = frost_debuff.AsGodotObject();
    }

    public override void _Draw()
    {
        if (this._max_area_range <= 0.0f)
        {
            return;
        }

        float radiusProgress = Mathf.Clamp(this._radius / this._max_area_range, 0.0f, 1.0f);
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

        Color waveColor = new(this.color.R, this.color.G, this.color.B, this.color.A * alphaFade);

        DrawSetTransform(Vector2.Zero, 0.0f, new Vector2(1.0f, this.y_scale));
        DrawCircle(Vector2.Zero, this._radius, new Color(waveColor.R, waveColor.G, waveColor.B, waveColor.A * 0.12f));
        DrawArc(Vector2.Zero, this._radius, 0, Mathf.Tau, 64, waveColor, this.thickness);
        DrawSetTransform(Vector2.Zero, 0.0f, Vector2.One);
    }

    private async void _on_body_entered(Node2D body)
    {
        Node2D enemy = body;
        if (!GodotObject.IsInstanceValid(enemy))
        {
            return;
        }

        CpuParticles2D explosion = BLUE_EXPLOSION.Instantiate<CpuParticles2D>();

        Enemy enemyModel = enemy as Enemy;
        enemyModel?.apply_damage(this._attack);
        enemy.Call("apply_debuff", this._frost_debuff);

        AddChild(explosion);
        explosion.GlobalPosition = enemy.GlobalPosition;
        explosion.Emitting = true;
        await ToSignal(explosion, CpuParticles2D.SignalName.Finished);
        explosion.QueueFree();
    }
}
