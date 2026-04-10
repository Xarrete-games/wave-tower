using Godot;

[GlobalClass]
public partial class BudaRelic : RelicRuntimeAdapter
{
    private readonly Buda _model = new();

    protected override RelicModel Model => this._model;

    public override void on_tower_placed(Variant towerInstance)
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject status = runContext?.Get("status").AsGodotObject();
        if (status == null)
        {
            return;
        }

        status.Set("max_health", (int)status.Get("max_health") + 1);
    }
}
