using Godot;

public class AttackRangeMultBuff : TowerBuffStatsModifier
{
    public AttackRangeMultBuff()
    {
    }

    public AttackRangeMultBuff(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, BuffData p_data = null, int p_value = 0)
        : base(p_source, p_duration, p_residual_buff, p_data, p_value)
    {
    }

    public override void contribute(TowerStatsAccumulator acc)
    {
        acc.attack_range_mult += this.value / 100.0f;
    }
}
