using Godot;

[GlobalClass]
public partial class LightningTower : Tower
{
    [Export] public PackedScene ElectricBallScene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || ElectricBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ElectricBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        projectile.set_target(_current_target, _get_attack());
    }
}
