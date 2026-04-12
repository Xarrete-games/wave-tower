using Godot;

[GlobalClass]
public partial class TowerBuff : RefCounted
{
    public Source source;
    public Variant data;
    public Duration duration;
    public TowerBuff residual_buff;

    public TowerBuff()
    {
    }

    public TowerBuff(Source p_source, Duration p_duration = null, TowerBuff p_residual_buff = null, Variant p_data = default)
    {
        this.source = p_source;
        this.duration = p_duration;
        this.residual_buff = p_residual_buff;
        this.data = p_data;
    }
}
