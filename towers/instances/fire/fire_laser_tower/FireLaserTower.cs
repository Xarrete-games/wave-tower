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
        this.red_projectile = GetNode<FireLaserProjectiel>("FireLaserProjectile");
        this.execute_threshold = this.base_execute_threshold;
    }

    protected override async void _fire()
    {
        Enemy targetEnemy = this._current_target as Enemy;
        if (!GodotObject.IsInstanceValid(targetEnemy) || this.red_projectile == null)
        {
            return;
        }

        float hpPercent = targetEnemy.get_percentage_remaining_health();
        Attack nextAttack = hpPercent > this.execute_threshold ? this._get_attack() : this._get_letal_attack();
        EnemyDebuff debuff = this.apply_burn ? EnemyDebuff.create_burn(this.damage_source) : null;

        this.red_projectile.set_target(targetEnemy, nextAttack, debuff);
        this.cristal_light?.turn_on();

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        this.red_projectile.hit_target();
        this.red_projectile.stop();
        this.cristal_light?.turn_off();
    }

    private Attack _get_letal_attack()
    {
        Attack attack = this._get_attack();
        attack.damage = EXECUTE_DAMAGE;
        attack.is_execution = true;
        return attack;
    }
}
