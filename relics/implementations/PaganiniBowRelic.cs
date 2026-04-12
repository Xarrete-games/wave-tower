using Godot;

[GlobalClass]
public partial class PaganiniBowRelic : RelicRuntimeAdapter
{
    private const string BUFF_ID = "damage_mult_buff";
    private const int BUFF_VALUE = 10;
    private const int COUNTER_THRESHOLD = 4;

    private static readonly Script _towerBuffFactoryScript = GD.Load<Script>("res://towers/tower-buffs/tower_buff_factory.gd");

    private readonly PaganiniBow _model = new();

    protected override RelicModel Model => this._model;

    public override void on_tower_placed(Variant towerInstance)
    {
        this.counter += 1;
        if (this.counter < COUNTER_THRESHOLD)
        {
            return;
        }

        this.counter = 0;
        GodotObject tower = towerInstance.AsGodotObject();
        if (tower == null)
        {
            return;
        }

        Variant source = new Source(Source.SourceType.RELIC, this.id, this);
        Variant buff = _towerBuffFactoryScript.Call("create_from_id", BUFF_ID, source, BUFF_VALUE);
        if (buff.VariantType != Variant.Type.Nil)
        {
            tower.Call("add_buff", buff);
        }
    }
}
