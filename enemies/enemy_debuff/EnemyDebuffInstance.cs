using Godot;
public class EnemyDebuffInstance {
    public EnemyDebuff debuff;
    public float expire_time;
    public float next_tick_time;
    public EnemyDebuffInstance() {
    }
    public EnemyDebuffInstance(EnemyDebuff debuffObj) {
        float now = Time.GetTicksMsec() / 1000.0f;
        debuff = debuffObj;
        expire_time = now + debuffObj.duration;
        next_tick_time = now + debuffObj.tick_interval;
    }
}

