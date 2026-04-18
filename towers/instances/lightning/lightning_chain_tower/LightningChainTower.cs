using Godot;

[GlobalClass]
public partial class LightningChainTower : Tower
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

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || LightningChainProjectileScene == null)
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

        projectile.SetTarget(_current_target, _get_attack(), CurrentBounces);
    }
}
