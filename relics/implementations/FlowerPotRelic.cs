using Godot;

[GlobalClass]
public partial class FlowerPotRelic : RelicRuntimeAdapter
{
    private const int HEALTH_BONUS = 1;

    private readonly FlowerPot _model = new();

    protected override RelicModel Model => this._model;

    public override void on_wave_finished()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject status = runContext?.Get("status").AsGodotObject();
        status?.Call("heal", HEALTH_BONUS);
    }
}
