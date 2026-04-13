public class AttackContext
{
    public float extra_additive { get; set; }
    public float extra_multiplicative { get; set; }
    public float damage_cap { get; set; } = 9999f;
    public float extra_crit_chance { get; set; }

    public EnemyModel Target { get; }
    public AttackModel Attack { get; }
    public TowerModel Tower { get; }

    public float ExtraAdditive { get => this.extra_additive; set => this.extra_additive = value; }
    public float ExtraMultiplicative { get => this.extra_multiplicative; set => this.extra_multiplicative = value; }
    public float DamageCap { get => this.damage_cap; set => this.damage_cap = value; }
    public float ExtraCritChance { get => this.extra_crit_chance; set => this.extra_crit_chance = value; }

    public AttackContext(EnemyModel target, AttackModel attack, TowerModel tower)
    {
        this.Target = target;
        this.Attack = attack;
        this.Tower = tower;
    }

    public float RebuildAttack()
    {
        float damage = (this.Attack?.Damage ?? 0f) + this.extra_additive;
        damage *= 1f + this.extra_multiplicative;
        return System.MathF.Min(damage, this.damage_cap);
    }

    public float GetCritChance()
    {
        return (this.Attack?.CritChance ?? 0f) + this.extra_crit_chance;
    }

    public float rebuild_attack() => this.RebuildAttack();
    public float get_crit_chance() => this.GetCritChance();
}
