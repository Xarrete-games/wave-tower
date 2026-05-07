using Godot;

[GlobalClass]
public partial class TowerData : BaseData
{
    [ExportGroup("Tower")]
    [Export]
    public int Type { get; set; }

    [ExportGroup("Prices")]
    [Export]
    public int BuildPrice { get; set; } = 50;

    [Export]
    public int UpgradePrice { get; set; } = 30;

    [ExportGroup("Initial Stats")]
    [Export]
    public float BaseDamage { get; set; } = 5f;

    [Export]
    public float BaseAttackRange { get; set; } = 200f;

    [Export]
    public float BaseAttackSpeed { get; set; } = 1f;

    [Export]
    public float BaseCriticChance { get; set; }

    [Export]
    public float BaseCriticDamage { get; set; } = 50f;

    [ExportGroup("Stats Per Level")]
    [Export]
    public float DamagePerLevel { get; set; }

    [Export]
    public float AttackRangePerLevel { get; set; }

    [Export]
    public float AttackSpeedPerLevel { get; set; }

    [Export]
    public float CriticChancePerLevel { get; set; }

    [Export]
    public float CriticDamagePerLevel { get; set; }

    public TowerStats Stats { get; set; } = new TowerStats();
    public TowerStats StatsOnLevel { get; set; } = new TowerStats();

    public void Build()
    {
        Stats.Damage = BaseDamage;
        Stats.AttackRange = BaseAttackRange;
        Stats.AttackSpeed = BaseAttackSpeed;
        Stats.CritChance = BaseCriticChance;
        Stats.CritDamage = BaseCriticDamage;

        StatsOnLevel.Damage = DamagePerLevel;
        StatsOnLevel.AttackRange = AttackRangePerLevel;
        StatsOnLevel.AttackSpeed = AttackSpeedPerLevel;
        StatsOnLevel.CritChance = CriticChancePerLevel;
        StatsOnLevel.CritDamage = CriticDamagePerLevel;
    }
}
