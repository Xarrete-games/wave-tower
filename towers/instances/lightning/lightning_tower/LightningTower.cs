using Godot;

[GlobalClass]
public partial class LightningTower : Tower
{
    [Export] public PackedScene electric_ball_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.electric_ball_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = this.electric_ball_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        projectile.set_target(this._current_target, this._get_attack());
    }
}
