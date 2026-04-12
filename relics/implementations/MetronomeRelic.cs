using Godot;

[GlobalClass]
public partial class MetronomeRelic : RelicRuntimeAdapter
{
    private const string BUFF_ID = "attack_speed_mult_buff";
    private const int BUFF_VALUE = 5;
    private const int COUNTER_THRESHOLD = 3;

    private readonly Metronome _model = new();

    protected override RelicModel Model => this._model;

    public override void on_wave_finished()
    {
        this.counter += 1;
        if (this.counter < COUNTER_THRESHOLD)
        {
            return;
        }

        this.counter = 0;

        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject towersManager = runContext?.Get("towers_manager").AsGodotObject();
        if (towersManager == null)
        {
            return;
        }

        Godot.Collections.Array<Variant> towers = towersManager.Get("towers").AsGodotArray<Variant>();
        if (towers.Count == 0)
        {
            return;
        }

        int selectedIndex = (int)GD.Randi() % towers.Count;
        GodotObject tower = towers[selectedIndex].AsGodotObject();
        if (tower == null)
        {
            return;
        }

        Variant source = new Source(Source.SourceType.RELIC, this.id, this);
        Variant buff = TowerBuffFactory.create_from_id(BUFF_ID, source, BUFF_VALUE);
        if (buff.VariantType == Variant.Type.Nil)
        {
            return;
        }

        tower.Call("add_buff", buff);
    }
}
