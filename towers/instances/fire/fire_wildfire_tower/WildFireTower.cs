using Godot;

[GlobalClass]
public partial class WildFireTower : TowerNode
{
    [Export] public PackedScene ProjectileScene;

    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || ProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ProjectileScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        EnemyDebuff debuff = EnemyDebuff.CreateBurn(DamageSource);
        projectile.SetTarget(_currentTarget, GetAttack(), debuff);
    }
}
