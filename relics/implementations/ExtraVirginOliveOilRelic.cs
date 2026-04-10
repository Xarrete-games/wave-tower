using Godot;

[GlobalClass]
public partial class ExtraVirginOliveOilRelic : RelicRuntimeAdapter
{
    private const int BURN_DEBUFF_TYPE = 1;
    private const float EXTRA_DURATION = 1.0f;

    private readonly ExtraVirginOliveOil _model = new();

    protected override RelicModel Model => this._model;

    public override void on_debuff_applied(Variant context, Variant target)
    {
        GodotObject ctx = context.AsGodotObject();
        GodotObject debuff = ctx?.Get("debuff").AsGodotObject();
        if (debuff == null)
        {
            return;
        }

        if ((int)debuff.Get("type") == BURN_DEBUFF_TYPE)
        {
            debuff.Set("duration", (float)debuff.Get("duration") + EXTRA_DURATION);
        }
    }
}
