using Godot;

[GlobalClass]
public partial class LightningTower : Tower
{
    [Export] public PackedScene electric_ball_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || electric_ball_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = electric_ball_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        projectile.set_target(_current_target, _get_attack());
    }
}
