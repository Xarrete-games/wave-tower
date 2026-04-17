using Godot;

[GlobalClass]
public partial class TowerData : BaseData
{
    [ExportGroup("Tower")]
    [Export]
    public int type { get; set; }

    [ExportGroup("Prices")]
    [Export]
    public int build_price { get; set; } = 50;

    [Export]
    public int upgrade_price { get; set; } = 30;

    [ExportGroup("Initial Stats")]
    [Export]
    public float base_damage { get; set; } = 5f;

    [Export]
    public float base_attack_range { get; set; } = 200f;

    [Export]
    public float base_attack_speed { get; set; } = 1f;

    [Export]
    public float base_critic_chance { get; set; }

    [Export]
    public float base_critic_damage { get; set; } = 50f;

    [ExportGroup("Stats Per Level")]
    [Export]
    public float damage_per_level { get; set; }

    [Export]
    public float attack_range_per_level { get; set; }

    [Export]
    public float attack_speed_per_level { get; set; }

    [Export]
    public float critic_chance_per_level { get; set; }

    [Export]
    public float critic_damage_per_level { get; set; }

    public int TowerType
    {
        get => type;
        set => type = value;
    }

    public int BuildPrice
    {
        get => build_price;
        set => build_price = value;
    }

    public int UpgradePrice
    {
        get => upgrade_price;
        set => upgrade_price = value;
    }

    public float BaseDamage
    {
        get => base_damage;
        set => base_damage = value;
    }

    public float BaseAttackRange
    {
        get => base_attack_range;
        set => base_attack_range = value;
    }

    public float BaseAttackSpeed
    {
        get => base_attack_speed;
        set => base_attack_speed = value;
    }

    public float BaseCriticChance
    {
        get => base_critic_chance;
        set => base_critic_chance = value;
    }

    public float BaseCriticDamage
    {
        get => base_critic_damage;
        set => base_critic_damage = value;
    }

    public float DamagePerLevel
    {
        get => damage_per_level;
        set => damage_per_level = value;
    }

    public float AttackRangePerLevel
    {
        get => attack_range_per_level;
        set => attack_range_per_level = value;
    }

    public float AttackSpeedPerLevel
    {
        get => attack_speed_per_level;
        set => attack_speed_per_level = value;
    }

    public float CriticChancePerLevel
    {
        get => critic_chance_per_level;
        set => critic_chance_per_level = value;
    }

    public float CriticDamagePerLevel
    {
        get => critic_damage_per_level;
        set => critic_damage_per_level = value;
    }

    public TowerStats stats { get; set; } = new TowerStats();
    public TowerStats stats_on_level { get; set; } = new TowerStats();

    public void build()
    {
        stats.damage = base_damage;
        stats.attack_range = base_attack_range;
        stats.attack_speed = base_attack_speed;
        stats.critic_chance = base_critic_chance;
        stats.critic_damage = base_critic_damage;

        stats_on_level.damage = damage_per_level;
        stats_on_level.attack_range = attack_range_per_level;
        stats_on_level.attack_speed = attack_speed_per_level;
        stats_on_level.critic_chance = critic_chance_per_level;
        stats_on_level.critic_damage = critic_damage_per_level;
    }

    public void Build()
    {
        build();
    }
}
