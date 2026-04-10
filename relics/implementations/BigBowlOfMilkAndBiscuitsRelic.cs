using Godot;

[GlobalClass]
public partial class BigBowlOfMilkAndBiscuitsRelic : RelicRuntimeAdapter
{
    private const int MAX_HEALTH_BONUS = 10;

    private readonly BigBowlOfMilkAndBiscuits _model = new();

    protected override RelicModel Model => this._model;

    public override void on_obtain()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject status = runContext?.Get("status").AsGodotObject();
        if (status == null)
        {
            return;
        }

        int maxHealth = (int)status.Get("max_health") + MAX_HEALTH_BONUS;
        status.Set("max_health", maxHealth);
        status.Set("health", (int)status.Get("health") + maxHealth);
    }
}
