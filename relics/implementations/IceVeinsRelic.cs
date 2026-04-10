using Godot;

[GlobalClass]
public partial class IceVeinsRelic : RelicRuntimeAdapter
{
    private const int FROST_DEBUFF_TYPE = 0;

    private readonly IceVeins _model = new();

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
            ctx.Set("stacks", (int)ctx.Get("stacks") + 1);
        }
    }
}
