using Godot;

[GlobalClass]
public partial class DamageContext : RefCounted
{
    public Variant attack { get; set; }
    public Variant target { get; set; }

    public float extra_additive { get; set; }
    public float extra_multiplicative { get; set; }
    public float damage_cap { get; set; } = 9999f;

    public AttackModel Attack { get; }
    public EnemyModel Target { get; }

    public float ExtraAdditive { get => this.extra_additive; set => this.extra_additive = value; }
    public float ExtraMultiplicative { get => this.extra_multiplicative; set => this.extra_multiplicative = value; }
    public float DamageCap { get => this.damage_cap; set => this.damage_cap = value; }

    public DamageContext(AttackModel attack, EnemyModel target)
    {
        this.Attack = attack;
        this.Target = target;
    }

    public DamageContext(Variant p_attack, Variant p_target)
    {
        this.attack = p_attack;
        this.target = p_target;
    }

    public DamageContext(Attack p_attack, Enemy p_target)
    {
        this.attack = p_attack;
        this.target = p_target;
    }

    public float GetTotalDamage()
    {
        Attack legacyAttack = this.attack.As<Attack>();
        if (legacyAttack != null)
        {
            float legacyDamage = legacyAttack.damage;
            legacyDamage += this.extra_additive;
            legacyDamage *= 1f + this.extra_multiplicative;
            return System.MathF.Min(legacyDamage, this.damage_cap);
        }

        float damage = this.Attack?.Damage ?? 0f;
        damage += this.extra_additive;
        damage *= 1f + this.extra_multiplicative;
        return System.MathF.Min(damage, this.damage_cap);
    }

    public float get_total_damage() => this.GetTotalDamage();
}
