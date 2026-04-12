using Godot;

[GlobalClass]
public partial class ValveAmplifierRelic : RelicRuntimeAdapter
{
    private const string BUFF_ID = "attack_range_mult_buff";
    private const int BUFF_VALUE = 10;
    private const int COUNTER_THRESHOLD = 3;

    private readonly ValveAmplifier _model = new();

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
        Variant buff = TowerBuffFactory.create_from_id(BUFF_ID, source, BUFF_VALUE);
        if (buff.VariantType != Variant.Type.Nil)
        {
            tower.Call("add_buff", buff);
        }
    }
}
