using Godot;

[GlobalClass]
public partial class FrostSpearTower : Tower
{
    [Export] public PackedScene FrostSpearProjectileScene;
    [Export] public int DebuffStacks = 2;

    private Marker2D _projectileSpawnPos;

    public override void _Ready()
    {
        base._Ready();
        _projectileSpawnPos = GetNode<Marker2D>("ProjectileSpawnPos");
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

        projectile.GlobalPosition = _projectileSpawnPos.GlobalPosition;

        Attack attack = GetAttack();
        int enemyFrostStacks = targetEnemy.GetDebuffStacks(0);
        float damageMultiplier = 1.0f + enemyFrostStacks * 0.10f;
        attack.Damage *= damageMultiplier;

        EnemyDebuff debuff = EnemyDebuff.CreateFrost(DamageSource);
        projectile.SetTarget(targetEnemy, attack, debuff, DebuffStacks);
    }
}
