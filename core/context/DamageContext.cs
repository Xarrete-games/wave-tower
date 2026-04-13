public class DamageContext
{
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

    public float GetTotalDamage()
    {
        float damage = this.Attack?.Damage ?? 0f;
        damage += this.extra_additive;
        damage *= 1f + this.extra_multiplicative;
        return System.MathF.Min(damage, this.damage_cap);
    }

    public float get_total_damage() => this.GetTotalDamage();
}
