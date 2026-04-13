using Godot;

[GlobalClass]
public partial class FrostTower : Tower
{
    private static readonly PackedScene FrostBallScene = GD.Load<PackedScene>("uid://cibktj8x8j1t8");
    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target))
        {
            return;
        }

        SingleTargetProjectile projectile = FrostBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        Attack attack = this._get_attack();
        Variant debuff = EnemyDebuff.create_frost(this.damage_source);
        projectile.set_target(this._current_target, attack, debuff);
    }
}
