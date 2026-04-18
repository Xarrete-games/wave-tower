using Godot;

[GlobalClass]
public partial class BuffData : BaseData
{
    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public TowerBuff CreateItem(Source source, int value = 0)
    {
        if (source == null)
        {
            return null;
        }

        TowerBuffStatsModifier buff = Id switch
        {
            "attack_speed_mult_buff" => new AttackSpeedMultBuff(),
            "attack_range_mult_buff" => new AttackRangeMultBuff(),
            "damage_flat_buff" => new DamageFlatBuff(),
            "damage_mult_buff" => new DamageMultBuff(),
            _ => null,
        };

        if (buff == null)
        {
            GD.PushError($"[BuffData] Unknown buff id: {Id}");
            return null;
        }

        buff.source = source;
        buff.data = this;
        buff.value = value;
        return buff;
    }
}
