public class DamageContext
{
    public float ExtraAdditive { get; set; }
    public float ExtraMultiplicative { get; set; }
    public float DamageCap { get; set; } = 9999f;

    public AttackModel Attack { get; }
    public Enemy Target { get; }

    public DamageContext(AttackModel attack, Enemy target)
    {
        Attack = attack;
        Target = target;
    }

    public float GetTotalDamage()
    {
        float damage = Attack?.Damage ?? 0f;
        damage += ExtraAdditive;
        damage *= 1f + ExtraMultiplicative;
        return System.MathF.Min(damage, DamageCap);
    }
}
