using Godot;
public class EnemyDebuffInstance {
    public EnemyDebuff Debuff;
    public float ExpireTime;
    public float NextTickTime;
    public EnemyDebuffInstance() {
    }
    public EnemyDebuffInstance(EnemyDebuff debuffObj) {
        float now = Time.GetTicksMsec() / 1000.0f;
        Debuff = debuffObj;
        ExpireTime = now + debuffObj.Duration;
        NextTickTime = now + debuffObj.TickInterval;
    }
}


