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

    protected override void Fire()
    {
        if (!GodotObject.IsInstanceValid(_currentTarget) || FireBallScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = FireBallScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        Attack attack = GetAttack();
        EnemyDebuff debuff = apply_burn ? EnemyDebuff.CreateBurn(DamageSource) : null;
        projectile.SetTarget(_currentTarget, attack, debuff);
    }
}
