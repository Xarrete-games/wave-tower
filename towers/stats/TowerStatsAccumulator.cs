using Godot;

[GlobalClass]
public partial class TowerStatsAccumulator : RefCounted
{
    public float flat_damage = 0.0f;
    public float damage_mult = 0.0f;
    public float flat_attack_range = 0.0f;
    public float attack_range_mult = 0.0f;
    public float flat_attack_speed = 0.0f;
    public float attack_speed_mult = 0.0f;
    public float flat_critic_chance = 0.0f;
    public float critic_chance_mult = 0.0f;
    public float flat_critic_damage = 0.0f;
    public float critic_damage_mult = 0.0f;

    public TowerStatsAccumulator merge(TowerStatsAccumulator other)
    {
        TowerStatsAccumulator result = new TowerStatsAccumulator();
        result.flat_damage = this.flat_damage + other.flat_damage;
        result.damage_mult = this.damage_mult + other.damage_mult;
        result.flat_attack_range = this.flat_attack_range + other.flat_attack_range;
        result.attack_range_mult = this.attack_range_mult + other.attack_range_mult;
        result.flat_attack_speed = this.flat_attack_speed + other.flat_attack_speed;
        result.attack_speed_mult = this.attack_speed_mult + other.attack_speed_mult;
        result.flat_critic_chance = this.flat_critic_chance + other.flat_critic_chance;
        result.critic_chance_mult = this.critic_chance_mult + other.critic_chance_mult;
        result.flat_critic_damage = this.flat_critic_damage + other.flat_critic_damage;
        result.critic_damage_mult = this.critic_damage_mult + other.critic_damage_mult;
        return result;
    }
}
