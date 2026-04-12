using Godot;

[GlobalClass]
public partial class IgnitionVoltageRelic : TowerBuffRelicAdapter
{
    private const int TOWER_TYPE_LIGHTNING = 1;
    private const string BUFF_ID = "attack_speed_mult_buff";
    private const int BUFF_VALUE = 15;

    private readonly IgnitionVoltage _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        Variant source = new Source(Source.SourceType.RELIC, this.id, this);
        Variant towerBuff = TowerBuffFactory.create_from_id(BUFF_ID, source, BUFF_VALUE);
        if (towerBuff.VariantType != Variant.Type.Nil)
        {
            tower.Call("add_buff", towerBuff);
        }
    }

    protected override void RemoveBuff(GodotObject tower)
    {
        tower.Call("remove_buff", this.id);
    }

    protected override bool IsValidTower(GodotObject tower)
    {
        return (int)tower.Get("type") == TOWER_TYPE_LIGHTNING;
    }
}
