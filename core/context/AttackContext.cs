public sealed class AttackContext
{
    public EnemyModel Target { get; }
    public AttackModel Attack { get; }
    public TowerModel Tower { get; }

    public float ExtraAdditive { get; set; }
    public float ExtraMultiplicative { get; set; }
    public float DamageCap { get; set; } = 9999f;
    public float ExtraCritChance { get; set; }

    public AttackContext(EnemyModel target, AttackModel attack, TowerModel tower)
    {
        this.Target = target;
        this.Attack = attack;
        this.Tower = tower;
    }

    public float RebuildAttack()
    {
        float damage = Attack.Damage + ExtraAdditive;
        damage *= 1f + this.ExtraMultiplicative;
        return System.MathF.Min(damage, this.DamageCap);
    }

    public float GetCritChance()
    {
        return this.Attack.CritChance + this.ExtraCritChance;
    }
}
