using Godot;

public abstract class TowerBuffStatsModifier : TowerBuff
{
    public int value = 0;

    protected TowerBuffStatsModifier()
    {
    }

    protected TowerBuffStatsModifier(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, BuffData p_data = null, int p_value = 0)
        : base(p_source, p_duration, p_residual_buff, p_data)
    {
        this.value = p_value;
    }

    public abstract void contribute(TowerStatsAccumulator acc);
}
