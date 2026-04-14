using Godot;

[GlobalClass]
public partial class FireTower : Tower
{
    [Export] public PackedScene fire_ball_scene;

    public bool apply_burn = false;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.fire_ball_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = this.fire_ball_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        Attack attack = this._get_attack();
        EnemyDebuff debuff = this.apply_burn ? EnemyDebuff.create_burn(this.damage_source) : null;
        projectile.set_target(this._current_target, attack, debuff);
    }
}
