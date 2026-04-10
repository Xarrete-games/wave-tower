using Godot;

[GlobalClass]
public partial class IgnitionVoltageRelic : TowerBuffRelicAdapter
{
    private const int RELIC_SOURCE_TYPE = 0;
    private const int TOWER_TYPE_LIGHTNING = 1;
    private const string BUFF_ID = "attack_speed_mult_buff";
    private const int BUFF_VALUE = 15;

    private static readonly Script _towerBuffFactoryScript = GD.Load<Script>("res://towers/tower-buffs/tower_buff_factory.gd");
    private static readonly Script _sourceScript = GD.Load<Script>("res://core/source.gd");

    private readonly IgnitionVoltage _model = new();

    protected override RelicModel Model => this._model;

    protected override void AddBuff(GodotObject tower)
    {
        Variant source = _sourceScript.Call("new", RELIC_SOURCE_TYPE, this.id, this);
        Variant towerBuff = _towerBuffFactoryScript.Call("create_from_id", BUFF_ID, source, BUFF_VALUE);
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
