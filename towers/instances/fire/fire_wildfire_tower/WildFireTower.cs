using Godot;

[GlobalClass]
public partial class WildFireTower : Tower
{
    [Export] public PackedScene projectile_scene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || projectile_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = projectile_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        EnemyDebuff debuff = EnemyDebuff.create_burn(damage_source);
        projectile.set_target(_current_target, _get_attack(), debuff);
    }
}
