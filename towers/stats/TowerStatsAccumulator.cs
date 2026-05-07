public class TowerStatsAccumulator
{
    public float FlatDamage = 0.0f;
    public float DamageMult = 0.0f;
    public float FlatAttackRange = 0.0f;
    public float AttackRangeMult = 0.0f;
    public float FlatAttackSpeed = 0.0f;
    public float AttackSpeedMult = 0.0f;
    public float FlatCritChance = 0.0f;
    public float CritChanceMult = 0.0f;
    public float FlatCritDamage = 0.0f;
    public float CritDamageMult = 0.0f;

    public TowerStatsAccumulator Merge(TowerStatsAccumulator other)
    {
        TowerStatsAccumulator result = new TowerStatsAccumulator();
        result.FlatDamage = FlatDamage + other.FlatDamage;
        result.DamageMult = DamageMult + other.DamageMult;
        result.FlatAttackRange = FlatAttackRange + other.FlatAttackRange;
        result.AttackRangeMult = AttackRangeMult + other.AttackRangeMult;
        result.FlatAttackSpeed = FlatAttackSpeed + other.FlatAttackSpeed;
        result.AttackSpeedMult = AttackSpeedMult + other.AttackSpeedMult;
        result.FlatCritChance = FlatCritChance + other.FlatCritChance;
        result.CritChanceMult = CritChanceMult + other.CritChanceMult;
        result.FlatCritDamage = FlatCritDamage + other.FlatCritDamage;
        result.CritDamageMult = CritDamageMult + other.CritDamageMult;
        return result;
    }
}
