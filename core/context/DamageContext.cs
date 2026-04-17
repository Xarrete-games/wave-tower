public class DamageContext
{
    public float extra_additive { get; set; }
    public float extra_multiplicative { get; set; }
    public float damage_cap { get; set; } = 9999f;

    public AttackModel Attack { get; }
    public EnemyModel Target { get; }

    public float ExtraAdditive { get => extra_additive; set => extra_additive = value; }
    public float ExtraMultiplicative { get => extra_multiplicative; set => extra_multiplicative = value; }
    public float DamageCap { get => damage_cap; set => damage_cap = value; }

    public DamageContext(AttackModel attack, EnemyModel target)
    {
        Attack = attack;
        Target = target;
    }

    public float GetTotalDamage()
    {
        float damage = Attack?.Damage ?? 0f;
        damage += extra_additive;
        damage *= 1f + extra_multiplicative;
        return System.MathF.Min(damage, damage_cap);
    }

    public float get_total_damage() => GetTotalDamage();
}
