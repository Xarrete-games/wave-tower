public sealed class DamageContext
{
    public AttackModel Attack { get; }
    public EnemyModel Target { get; }

    public float ExtraAdditive { get; set; }
    public float ExtraMultiplicative { get; set; }
    public float DamageCap { get; set; } = 9999f;

    public DamageContext(AttackModel attack, EnemyModel target)
    {
        this.Attack = attack;
        this.Target = target;
    }

    public float GetTotalDamage()
    {
        float damage = this.Attack.Damage;
        damage += this.ExtraAdditive;
        damage *= 1f + this.ExtraMultiplicative;
        return System.MathF.Min(damage, this.DamageCap);
    }
}
