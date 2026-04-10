using Godot;

[GlobalClass]
public partial class ArticCubeRelic : RelicRuntimeAdapter
{
    private const int FROST_DEBUFF_TYPE = 0;
    private const float EXTRA_DURATION = 1.0f;

    private readonly ArticCube _model = new();

    protected override RelicModel Model => this._model;

    public override void on_debuff_applied(Variant context, Variant target)
    {
        GodotObject ctx = context.AsGodotObject();
        GodotObject debuff = ctx?.Get("debuff").AsGodotObject();
        if (debuff == null)
        {
            return;
        }

        if ((int)debuff.Get("type") == FROST_DEBUFF_TYPE)
        {
            debuff.Set("duration", (float)debuff.Get("duration") + EXTRA_DURATION);
        }
    }
}
