public class TowerStatsAccumulator
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
        result.flat_damage = flat_damage + other.flat_damage;
        result.damage_mult = damage_mult + other.damage_mult;
        result.flat_attack_range = flat_attack_range + other.flat_attack_range;
        result.attack_range_mult = attack_range_mult + other.attack_range_mult;
        result.flat_attack_speed = flat_attack_speed + other.flat_attack_speed;
        result.attack_speed_mult = attack_speed_mult + other.attack_speed_mult;
        result.flat_critic_chance = flat_critic_chance + other.flat_critic_chance;
        result.critic_chance_mult = critic_chance_mult + other.critic_chance_mult;
        result.flat_critic_damage = flat_critic_damage + other.flat_critic_damage;
        result.critic_damage_mult = critic_damage_mult + other.critic_damage_mult;
        return result;
    }
}
