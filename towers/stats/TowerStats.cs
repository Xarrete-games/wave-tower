public class TowerStats
{
    public float Damage { get; set; }
    public float AttackRange { get; set; }
    public float AttackSpeed { get; set; }
    public float CritChance { get; set; }
    public float CritDamage { get; set; }

    public TowerStats Duplicate()
    {
        return new TowerStats
        {
            Damage = Damage,
            AttackRange = AttackRange,
            AttackSpeed = AttackSpeed,
            CritChance = CritChance,
            CritDamage = CritDamage,
        };
    }

    public void AddStats(TowerStats other)
    {
        Damage += other.Damage;
        AttackRange += other.AttackRange;
        AttackSpeed += other.AttackSpeed;
        CritChance += other.CritChance;
        CritDamage += other.CritDamage;
    }
}
