using Godot;

[GlobalClass]
public partial class PirateHatRelic : RelicRuntimeAdapter
{
    private const double CHANCE_TO_RECOVER_CONSUMABLE = 0.5;

    private readonly PirateHat _model = new();
    private readonly System.Random _random = new();

    protected override RelicModel Model => this._model;

    public override void on_consumable_used(Variant consumable)
    {
        if (consumable.VariantType == Variant.Type.Nil)
        {
            return;
        }

        if (this._random.NextDouble() >= CHANCE_TO_RECOVER_CONSUMABLE)
        {
            return;
        }

        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject consumablesManager = runContext?.Get("consumables_manager").AsGodotObject();
        consumablesManager?.Call("add_consumable", consumable);
    }
}
