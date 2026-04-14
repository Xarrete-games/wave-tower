using Godot;

public class DamageFlatBuff : TowerBuffStatsModifier
{
    public DamageFlatBuff()
    {
    }

    public DamageFlatBuff(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, BuffData p_data = null, int p_value = 0)
        : base(p_source, p_duration, p_residual_buff, p_data, p_value)
    {
    }

    public override void contribute(TowerStatsAccumulator acc)
    {
        acc.flat_damage += this.value;
    }
}
