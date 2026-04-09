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

    public TowerStats stats { get; set; } = new TowerStats();
    public TowerStats stats_on_level { get; set; } = new TowerStats();

    public void build()
    {
        this.stats.damage = this.base_damage;
        this.stats.attack_range = this.base_attack_range;
        this.stats.attack_speed = this.base_attack_speed;
        this.stats.critic_chance = this.base_critic_chance;
        this.stats.critic_damage = this.base_critic_damage;

        this.stats_on_level.damage = this.damage_per_level;
        this.stats_on_level.attack_range = this.attack_range_per_level;
        this.stats_on_level.attack_speed = this.attack_speed_per_level;
        this.stats_on_level.critic_chance = this.critic_chance_per_level;
        this.stats_on_level.critic_damage = this.critic_damage_per_level;
    }
}
