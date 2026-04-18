using Godot;

[GlobalClass]
public partial class FireTower : Tower
{
    [Export] public PackedScene FireBallScene;

    public bool apply_burn = false;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(_current_target) || FireBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = FireBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        Attack attack = _get_attack();
        EnemyDebuff debuff = apply_burn ? EnemyDebuff.create_burn(damage_source) : null;
        projectile.set_target(_current_target, attack, debuff);
    }
}
