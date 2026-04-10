using Godot;

[GlobalClass]
public partial class PowerGlovesRelic : RelicRuntimeAdapter
{
    private const int TARGET_TOWER = 1;
    private const int RELIC_SOURCE_TYPE = 0;
    private const string BUFF_ID = "damage_flat_buff";
    private const int BUFF_VALUE = 3;

    private static readonly Script _towerBuffFactoryScript = GD.Load<Script>("res://towers/tower-buffs/tower_buff_factory.gd");
    private static readonly Script _sourceScript = GD.Load<Script>("res://core/source.gd");

    private readonly PowerGloves _model = new();

    protected override RelicModel Model => this._model;

    public override void on_consumable_used(Variant consumable)
    {
        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj == null)
        {
            return;
        }

        bool requiresTarget = consumableObj.HasMethod("requires_target") && (bool)consumableObj.Call("requires_target");
        if (!requiresTarget)
        {
            return;
        }

        GodotObject data = consumableObj.Get("data").AsGodotObject();
        if (data == null || (int)data.Get("targeting_type") != TARGET_TOWER)
        {
            return;
        }

        GodotObject tower = consumableObj.Get("target").AsGodotObject();
        if (tower == null)
        {
            return;
        }

        Variant source = _sourceScript.Call("new", RELIC_SOURCE_TYPE, this.id, this);
        Variant buff = _towerBuffFactoryScript.Call("create_from_id", BUFF_ID, source, BUFF_VALUE);
        if (buff.VariantType == Variant.Type.Nil)
        {
            return;
        }

        tower.Call("add_buff", buff);
    }
}
