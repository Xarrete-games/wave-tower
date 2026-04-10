using Godot;

[GlobalClass]
public partial class SafetyHelmetRelic : RelicRuntimeAdapter
{
    private const int ARMOR_AMOUNT = 1;

    private readonly SafetyHelmet _model = new();

    protected override RelicModel Model => this._model;

    public override void on_wave_init()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject status = runContext?.Get("status").AsGodotObject();
        status?.Call("add_amor", ARMOR_AMOUNT);
    }
}
