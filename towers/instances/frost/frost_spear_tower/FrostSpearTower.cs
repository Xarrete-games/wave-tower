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

    protected override void _fire()
    {
        Enemy targetEnemy = _current_target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || FrostSpearProjectileScene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = FrostSpearProjectileScene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = projectile_spawn_pos.GlobalPosition;

        Attack attack = _get_attack();
        int enemyFrostStacks = targetEnemy.get_debuff_stacks(0);
        float damageMultiplier = 1.0f + enemyFrostStacks * 0.10f;
        attack.damage *= damageMultiplier;

        EnemyDebuff debuff = EnemyDebuff.create_frost(damage_source);
        projectile.set_target(targetEnemy, attack, debuff, DebuffStacks);
    }
}
