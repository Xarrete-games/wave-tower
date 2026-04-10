using Godot;

[GlobalClass]
public partial class SaltRelic : RelicRuntimeAdapter
{
    private const float LOW_HEALTH_THRESHOLD = 30f;
    private const float BONUS_MULTIPLIER = 0.3f;

    private readonly Salt _model = new();

    protected override RelicModel Model => this._model;

    public override void on_before_damage(Variant context)
    {
        GodotObject contextObj = context.AsGodotObject();
        GodotObject target = contextObj?.Get("target").AsGodotObject();
        if (target == null)
        {
            return;
        }

        float healthPercent = (float)target.Call("get_percentage_remaining_health");
        if (healthPercent > LOW_HEALTH_THRESHOLD)
        {
            return;
        }

        float current = (float)contextObj.Get("extra_multiplicative");
        contextObj.Set("extra_multiplicative", current + BONUS_MULTIPLIER);
    }
}
