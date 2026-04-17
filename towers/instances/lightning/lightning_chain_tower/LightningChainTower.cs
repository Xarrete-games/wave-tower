using Godot;

[GlobalClass]
public partial class LightningChainTower : Tower
{
    [Export] public PackedScene lightning_chain_projectile_scene;

    public int base_bounces = 3;
    public int current_bounces = 3;

    private Marker2D projectile_spawn_point;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_point = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || lightning_chain_projectile_scene == null)
        {
            return;
        }

        LightningChainProjectile projectile = lightning_chain_projectile_scene.Instantiate<LightningChainProjectile>();
        CallDeferred(MethodName.FireChain, projectile);
    }

    private void FireChain(LightningChainProjectile projectile)
    {
        AddChild(projectile);
        projectile.GlobalPosition = projectile_spawn_point.GlobalPosition;

        projectile.set_target(_current_target, _get_attack(), current_bounces);
    }
}
