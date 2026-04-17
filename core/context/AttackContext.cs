public class AttackContext
{
    public float extra_additive { get; set; }
    public float extra_multiplicative { get; set; }
    public float damage_cap { get; set; } = 9999f;
    public float extra_crit_chance { get; set; }

    public EnemyModel Target { get; }
    public AttackModel Attack { get; }
    public TowerModel Tower { get; }

    public float ExtraAdditive { get => extra_additive; set => extra_additive = value; }
    public float ExtraMultiplicative { get => extra_multiplicative; set => extra_multiplicative = value; }
    public float DamageCap { get => damage_cap; set => damage_cap = value; }
    public float ExtraCritChance { get => extra_crit_chance; set => extra_crit_chance = value; }

    public AttackContext(EnemyModel target, AttackModel attack, TowerModel tower)
    {
        Target = target;
        Attack = attack;
        Tower = tower;
    }

    public float RebuildAttack()
    {
        float damage = (Attack?.Damage ?? 0f) + extra_additive;
        damage *= 1f + extra_multiplicative;
        return System.MathF.Min(damage, damage_cap);
    }

    public float GetCritChance()
    {
        return (Attack?.CritChance ?? 0f) + extra_crit_chance;
    }

    public float rebuild_attack() => RebuildAttack();
    public float get_crit_chance() => GetCritChance();
}
