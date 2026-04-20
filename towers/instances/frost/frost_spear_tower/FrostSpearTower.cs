using Godot;

[GlobalClass]
public partial class FrostSpearTower : Tower
{
    [Export] public PackedScene FrostSpearProjectileScene;
    [Export] public int DebuffStacks = 2;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void Fire()
    {
        Enemy targetEnemy = _currentTarget as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || FrostSpearProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = FrostSpearProjectileScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        Attack attack = GetAttack();
        int enemyFrostStacks = targetEnemy.GetDebuffStacks(0);
        float damageMultiplier = 1.0f + enemyFrostStacks * 0.10f;
        attack.Damage *= damageMultiplier;

        EnemyDebuff debuff = EnemyDebuff.CreateFrost(DamageSource);
        projectile.SetTarget(targetEnemy, attack, debuff, DebuffStacks);
    }
}
