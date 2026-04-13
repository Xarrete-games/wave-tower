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
            damage = this.damage,
            attack_range = this.attack_range,
            attack_speed = this.attack_speed,
            critic_chance = this.critic_chance,
            critic_damage = this.critic_damage,
        };
    }

    public void add_stats(TowerStats other)
    {
        this.damage += other.damage;
        this.attack_range += other.attack_range;
        this.attack_speed += other.attack_speed;
        this.critic_chance += other.critic_chance;
        this.critic_damage += other.critic_damage;
    }
}
