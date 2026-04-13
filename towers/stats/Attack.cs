using Godot;
using System.Collections.Generic;

public class Attack
{
    public float damage;
    public bool is_critical = false;
    public bool is_execution = false;
    public int hits = 1;
    public int bounces = 0;
    public List<Variant> effects = new();
    public Source source;
    public Dictionary<string, Variant> tags = new();
    public float crit_chance = 0.0f;

    public Attack()
    {
    }

    public Attack(float p_damage, Source p_source, bool p_is_critical = false, bool p_is_execution = false, Dictionary<string, Variant> p_tags = null)
    {
        this.damage = p_damage;
        this.is_critical = p_is_critical;
        this.is_execution = p_is_execution;
        this.source = p_source;
        this.tags = p_tags ?? new Dictionary<string, Variant>();
    }
}
