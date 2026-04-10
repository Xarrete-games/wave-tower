using Godot;

[GlobalClass]
public partial class PiratePatchRelic : RelicRuntimeAdapter
{
    private const int HEALTH_AMOUNT = 2;

    private readonly PiratePatch _model = new();

    protected override RelicModel Model => this._model;

    public override void on_consumable_used(Variant consumable)
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject status = runContext?.Get("status").AsGodotObject();
        status?.Call("heal", HEALTH_AMOUNT);
    }
}
