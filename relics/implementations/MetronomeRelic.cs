using Godot;

[GlobalClass]
public partial class MetronomeRelic : RelicRuntimeAdapter
{
    private const int RELIC_SOURCE_TYPE = 0;
    private const string BUFF_ID = "attack_speed_mult_buff";
    private const int BUFF_VALUE = 5;
    private const int COUNTER_THRESHOLD = 3;

    private static readonly Script _towerBuffFactoryScript = GD.Load<Script>("res://towers/tower-buffs/tower_buff_factory.gd");
    private static readonly Script _sourceScript = GD.Load<Script>("res://core/source.gd");

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

        Variant source = _sourceScript.Call("new", RELIC_SOURCE_TYPE, this.id, this);
        Variant buff = _towerBuffFactoryScript.Call("create_from_id", BUFF_ID, source, BUFF_VALUE);
        if (buff.VariantType == Variant.Type.Nil)
        {
            return;
        }

        tower.Call("add_buff", buff);
    }
}
