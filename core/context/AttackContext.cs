public class AttackContext
{
    public float ExtraAdditive { get; set; }
    public float ExtraMultiplicative { get; set; }
    public float DamageCap { get; set; } = 9999f;
    public float ExtraCritChance { get; set; }

    public Enemy Target { get; }
    public AttackModel Attack { get; }
    public TowerModel Tower { get; }

    public AttackContext(Enemy target, AttackModel attack, TowerModel tower)
    {
        Target = target;
        Attack = attack;
        Tower = tower;
    }

    public float RebuildAttack()
    {
        float damage = (Attack?.Damage ?? 0f) + ExtraAdditive;
        damage *= 1f + ExtraMultiplicative;
        return System.MathF.Min(damage, DamageCap);
    }

    public float GetCritChance()
    {
        return (Attack?.CritChance ?? 0f) + ExtraCritChance;
    }
}
