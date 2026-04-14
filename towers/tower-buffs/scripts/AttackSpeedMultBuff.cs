public class AttackSpeedMultBuff : TowerBuffStatsModifier
{
    public AttackSpeedMultBuff()
    {
    }

    public AttackSpeedMultBuff(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, BuffData p_data = null, int p_value = 0)
        : base(p_source, p_duration, p_residual_buff, p_data, p_value)
    {
    }

    public override void contribute(TowerStatsAccumulator acc)
    {
        acc.attack_speed_mult += this.value / 100.0f;
    }
}
