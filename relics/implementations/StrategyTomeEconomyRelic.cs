using Godot;

[GlobalClass]
public partial class StrategyTomeEconomyRelic : RelicRuntimeAdapter
{
    private readonly StrategyTomeEconomy _model = new();

    protected override RelicModel Model => this._model;

    public override void on_obtain()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject economy = runContext?.Get("economy").AsGodotObject();
        economy?.Set("is_sell_active", true);
    }

    public override void on_remove()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject economy = runContext?.Get("economy").AsGodotObject();
        economy?.Set("is_sell_active", false);
    }
}
