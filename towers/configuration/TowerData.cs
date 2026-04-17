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

    public int type
    {
        get => Type;
        set => Type = value;
    }

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

    public TowerStats stats { get; set; } = new TowerStats();
    public TowerStats stats_on_level { get; set; } = new TowerStats();

    public void build()
    {
        stats.damage = BaseDamage;
        stats.attack_range = BaseAttackRange;
        stats.attack_speed = BaseAttackSpeed;
        stats.critic_chance = BaseCriticChance;
        stats.critic_damage = BaseCriticDamage;

        stats_on_level.damage = DamagePerLevel;
        stats_on_level.attack_range = AttackRangePerLevel;
        stats_on_level.attack_speed = AttackSpeedPerLevel;
        stats_on_level.critic_chance = CriticChancePerLevel;
        stats_on_level.critic_damage = CriticDamagePerLevel;
    }

    public void Build()
    {
        build();
    }
}
