using Godot;

[GlobalClass]
public partial class FireLaserTower : Tower
{
    private const float EXECUTE_DAMAGE = 9999f;

    public float execute_threshold = 0.0f;
    public float base_execute_threshold = 10.0f;
    public bool apply_burn = false;

    private FireLaserProjectiel red_projectile;

    public override void _Ready()
    {
        base._Ready();
        red_projectile = GetNode<FireLaserProjectiel>("FireLaserProjectile");
        execute_threshold = base_execute_threshold;
    }

    protected override async void Fire()
    {
        Enemy targetEnemy = _currentTarget as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || red_projectile == null)
        {
            return;
        }

        float hpPercent = targetEnemy.GetPercentageRemainingHealth();
        Attack nextAttack = hpPercent > execute_threshold ? GetAttack() : GetLetalAttack();
        EnemyDebuff debuff = apply_burn ? EnemyDebuff.CreateBurn(DamageSource) : null;

        red_projectile.SetTarget(targetEnemy, nextAttack, debuff);
        _cristalLight?.turn_on();

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        red_projectile.HitTarget();
        red_projectile.Stop();
        _cristalLight?.turn_off();
    }

    private Attack GetLetalAttack()
    {
        Attack attack = GetAttack();
        attack.Damage = EXECUTE_DAMAGE;
        attack.IsExecution = true;
        return attack;
    }
}
