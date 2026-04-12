using Godot;

[GlobalClass]
public abstract partial class TowerBuffStatsModifier : TowerBuff
{
    public int value = 0;

    protected TowerBuffStatsModifier()
    {
    }

    protected TowerBuffStatsModifier(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, Variant p_data = default, int p_value = 0)
        : base(p_source, p_duration, p_residual_buff, p_data)
    {
        this.value = p_value;
    }

    public abstract void contribute(TowerStatsAccumulator acc);
}
