using Godot;

[GlobalClass]
public partial class WildFireTower : Tower
{
    [Export] public PackedScene projectile_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.projectile_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = this.projectile_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        projectile.set_target(this._current_target, this._get_attack(), EnemyDebuff.create_burn(this.damage_source));
    }
}
