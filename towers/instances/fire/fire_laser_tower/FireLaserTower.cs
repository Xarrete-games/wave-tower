using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class FireLaserTower : Tower
{
    private const float ExecuteDamage = 9999f;

    public float ExecuteThreshold = 0.0f;
    public float BaseExecuteThreshold = 10.0f;
    public bool ApplyBurn = false;

    private FireLaserProjectiel _redProjectile;

    public override void _Ready()
    {
        base._Ready();
        _redProjectile = GetNode<FireLaserProjectiel>("FireLaserProjectile");
        ExecuteThreshold = BaseExecuteThreshold;
    }

    protected override void Fire()
    {
        _ = FireAsync();
    }

    private async Task FireAsync()
    {
        Enemy targetEnemy = _currentTarget as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || _redProjectile == null)
        {
            return;
        }

        float hpPercent = targetEnemy.GetPercentageRemainingHealth();
        Attack nextAttack = hpPercent > ExecuteThreshold ? GetAttack() : GetLethalAttack();
        EnemyDebuff debuff = ApplyBurn ? EnemyDebuff.CreateBurn(DamageSource) : null;

        _redProjectile.SetTarget(targetEnemy, nextAttack, debuff);
        _cristalLight?.TurnOn();

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        _redProjectile.HitTarget();
        _redProjectile.Stop();
        _cristalLight?.TurnOff();
    }

    private Attack GetLethalAttack()
    {
        Attack attack = GetAttack();
        attack.Damage = ExecuteDamage;
        attack.IsExecution = true;
        return attack;
    }
}
