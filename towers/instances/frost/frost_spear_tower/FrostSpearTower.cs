using Godot;

[GlobalClass]
public partial class FrostSpearTower : Tower
{
    [Export] public PackedScene frost_spear_projectile_scene;
    [Export] public int debuff_stacks = 2;

    private Marker2D projectile_spawn_pos;

    public override void _Ready()
    {
        base._Ready();
        this.projectile_spawn_pos = GetNode<Marker2D>("ProjectileSpawnPos");
    }

    protected override void _fire()
    {
        Enemy targetEnemy = this._current_target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || this.frost_spear_projectile_scene == null)
        {
            return;
        }

        SingleTargetProjectile projectile = this.frost_spear_projectile_scene.Instantiate<SingleTargetProjectile>();
        AddChild(projectile);

        projectile.GlobalPosition = this.projectile_spawn_pos.GlobalPosition;

        Attack attack = this._get_attack();
        int enemyFrostStacks = targetEnemy.get_debuff_stacks(0);
        float damageMultiplier = 1.0f + enemyFrostStacks * 0.10f;
        attack.damage *= damageMultiplier;

        EnemyDebuff debuff = EnemyDebuff.create_frost(this.damage_source);
        projectile.set_target(targetEnemy, attack, debuff, this.debuff_stacks);
    }
}
