using Godot;

[GlobalClass]
public partial class BlackWitchHatRelic : RelicRuntimeAdapter
{
    private const int SOURCE_TYPE_TOWER = 1;
    private const float BONUS_DAMAGE = 5.0f;

    private readonly BlackWitchHat _model = new();

    protected override RelicModel Model => this._model;

    public override void on_before_damage(Variant context)
    {
        GodotObject ctx = context.AsGodotObject();
        GodotObject target = ctx?.Get("target").AsGodotObject();
        GodotObject attack = ctx?.Get("attack").AsGodotObject();
        GodotObject source = attack?.Get("source").AsGodotObject();
        if (target == null || source == null)
        {
            return;
        }

        bool hasDebuff = (bool)target.Call("has_any_debuff");
        int sourceType = (int)source.Get("type");
        if (!hasDebuff || sourceType != SOURCE_TYPE_TOWER)
        {
            return;
        }

        float extra = (float)ctx.Get("extra_additive");
        ctx.Set("extra_additive", extra + BONUS_DAMAGE);
    }
}
