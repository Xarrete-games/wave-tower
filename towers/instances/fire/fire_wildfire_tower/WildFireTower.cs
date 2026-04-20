using Godot;

[GlobalClass]
public partial class WildFireTower : Tower
{
    [Export] public PackedScene ProjectileScene;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || ProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = ProjectileScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        EnemyDebuff debuff = EnemyDebuff.CreateBurn(DamageSource);
        projectile.SetTarget(_currentTarget, GetAttack(), debuff);
    }
}
