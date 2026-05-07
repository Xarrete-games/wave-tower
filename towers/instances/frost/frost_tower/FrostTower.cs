using Godot;

[GlobalClass]
public partial class FrostTower : Tower
{
    private static readonly PackedScene FrostBallScene = GD.Load<PackedScene>("uid://cibktj8x8j1t8");
    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget))
        {
            return;
        }

        SingleTargetProjectile projectile = FrostBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        Attack attack = GetAttack();
        EnemyDebuff debuff = EnemyDebuff.CreateFrost(DamageSource);
        projectile.SetTarget(_currentTarget, attack, debuff);
    }
}
