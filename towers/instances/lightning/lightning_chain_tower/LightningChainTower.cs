using Godot;

[GlobalClass]
public partial class LightningChainTower : TowerNode
{
    [Export] public PackedScene LightningChainProjectileScene;

    public int BaseBounces = 3;
    public int CurrentBounces = 3;

    private Marker2D _projectileSpawnPoint;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPoint = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || LightningChainProjectileScene == null)
        {
            return;
        }

        LightningChainProjectile projectile = LightningChainProjectileScene.Instantiate<LightningChainProjectile>();
        CallDeferred(MethodName.FireChain, projectile);
    }

    private void FireChain(LightningChainProjectile projectile)
    {
        AddChild(projectile);
        projectile.GlobalPosition = _projectileSpawnPoint.GlobalPosition;

        projectile.SetTarget(_currentTarget, GetAttack(), CurrentBounces);
    }
}
