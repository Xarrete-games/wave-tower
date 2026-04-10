using Godot;

[GlobalClass]
public partial class TunelVisionRelic : RelicRuntimeAdapter
{
    private const float DAMAGE_MULTIPLIER_PER_HIT = 0.05f;

    private readonly TunelVision _model = new();
    private readonly Godot.Collections.Dictionary<GodotObject, GodotObject> _lastTargetByTower = new();
    private readonly Godot.Collections.Dictionary<GodotObject, int> _hitCountByTower = new();

    protected override RelicModel Model => this._model;

    public override void on_before_attack(Variant context)
    {
        GodotObject ctx = context.AsGodotObject();
        GodotObject tower = ctx?.Get("tower").AsGodotObject();
        GodotObject target = ctx?.Get("target").AsGodotObject();
        if (tower == null)
        {
            return;
        }

        if (!this._lastTargetByTower.ContainsKey(tower))
        {
            this._lastTargetByTower[tower] = null;
            this._hitCountByTower[tower] = 0;
        }

        GodotObject lastTarget = this._lastTargetByTower[tower];
        if (target == lastTarget)
        {
            this._hitCountByTower[tower] = this._hitCountByTower[tower] + 1;
        }
        else
        {
            this._lastTargetByTower[tower] = target;
            this._hitCountByTower[tower] = 0;
        }

        float extra = (float)ctx.Get("extra_multiplicative");
        ctx.Set("extra_multiplicative", extra + (DAMAGE_MULTIPLIER_PER_HIT * this._hitCountByTower[tower]));
    }
}
