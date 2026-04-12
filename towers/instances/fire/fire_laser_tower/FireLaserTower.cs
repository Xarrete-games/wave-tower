using Godot;

[GlobalClass]
public partial class FireLaserTower : Tower
{
    private static readonly Script EnemyDebuffScript = GD.Load<Script>("res://enemies/enemy_debuff/enemy_debuff.gd");

    private const float EXECUTE_DAMAGE = 9999f;

    public float execute_threshold = 0.0f;
    public float base_execute_threshold = 10.0f;
    public bool apply_burn = false;

    private Node red_projectile;

    public override void _Ready()
    {
        base._Ready();
        this.red_projectile = GetNode("FireLaserProjectile");
        this.execute_threshold = this.base_execute_threshold;
    }

    protected override async void _fire()
    {
        if (!GodotObject.IsInstanceValid(this._current_target) || this.red_projectile == null)
        {
            return;
        }

        float hpPercent = this._current_target.Call("get_percentage_remaining_health").AsSingle();
        GodotObject nextAttack = hpPercent > this.execute_threshold ? this._get_attack() : this._get_letal_attack();
        Variant debuff = this.apply_burn ? EnemyDebuffScript.Call("create_burn", this.damage_source) : default;

        this.red_projectile.Call("set_target", this._current_target, nextAttack, debuff);
        this.cristal_light?.turn_on();

        await ToSignal(GetTree().CreateTimer(0.1f, false), Timer.SignalName.Timeout);

        this.red_projectile.Call("hit_target");
        this.red_projectile.Call("stop");
        this.cristal_light?.turn_off();
    }

    private GodotObject _get_letal_attack()
    {
        GodotObject attack = this._get_attack();
        attack.Set("damage", EXECUTE_DAMAGE);
        attack.Set("is_execution", true);
        return attack;
    }
}
