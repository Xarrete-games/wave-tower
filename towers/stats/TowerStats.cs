public class TowerStats
{
    public float damage { get; set; }
    public float attack_range { get; set; }
    public float attack_speed { get; set; }
    public float critic_chance { get; set; }
    public float critic_damage { get; set; }

    public TowerStats duplicate()
    {
        return new TowerStats
        {
            damage = damage,
            attack_range = attack_range,
            attack_speed = attack_speed,
            critic_chance = critic_chance,
            critic_damage = critic_damage,
        };
    }

    public void add_stats(TowerStats other)
    {
        damage += other.damage;
        attack_range += other.attack_range;
        attack_speed += other.attack_speed;
        critic_chance += other.critic_chance;
        critic_damage += other.critic_damage;
    }
}
