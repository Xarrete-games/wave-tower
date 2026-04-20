using Godot;

[GlobalClass]
public partial class FrostNovaTower : Tower
{
    private static readonly PackedScene FrostNovaProjectileScene = GD.Load<PackedScene>("uid://csif0nju31dcs");
    public float double_shot_chance = 0;

    private Marker2D projectil_spawn_point;

    public override void _Ready()
    {
        base._Ready();
        projectil_spawn_point = GetNode<Marker2D>("ProjectilSpawnPoint");
    }

    protected override async void _fire()
    {
        cristal_light?.play();

        BlueProjectil projectile = FrostNovaProjectileScene.Instantiate<BlueProjectil>();
        bool isDoubleHit = IsDobleHit();

        Attack attack = _get_attack();
        float attackRange = stats?.attack_range ?? 0f;
        projectile.set_stats(attack, attackRange, EnemyDebuff.CreateFrost(DamageSource));
        CallDeferred(MethodName.AddProjectil, projectile);

        if (!isDoubleHit)
        {
            return;
        }

        await ToSignal(GetTree().CreateTimer(0.5f, false), Timer.SignalName.Timeout);

        cristal_light?.play();
        projectile = FrostNovaProjectileScene.Instantiate<BlueProjectil>();
        attack = _get_attack();
        projectile.set_stats(attack, attackRange, EnemyDebuff.CreateFrost(DamageSource));
        CallDeferred(MethodName.AddProjectil, projectile);
    }

    private void AddProjectil(Node projectile)
    {
        AddChild(projectile);
        if (projectile is Node2D node2D)
        {
            node2D.Position = projectil_spawn_point.Position;
        }
    }

    private bool IsDobleHit()
    {
        float random = GD.Randf();
        return (double_shot_chance / 100f) >= random;
    }
}
