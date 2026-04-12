using Godot;

[GlobalClass]
public partial class EnemyDebuffInstance : RefCounted
{
    public EnemyDebuff debuff;
    public float expire_time;
    public float next_tick_time;

    public EnemyDebuffInstance()
    {
    }

    public EnemyDebuffInstance(EnemyDebuff p_debuff)
    {
        float now = Time.GetTicksMsec() / 1000.0f;
        this.debuff = p_debuff;
        this.expire_time = now + p_debuff.duration;
        this.next_tick_time = now + p_debuff.tick_interval;
    }
}
