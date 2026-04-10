using Godot;

[GlobalClass]
public partial class RunicLighterRelic : RelicRuntimeAdapter
{
    private const int SOURCE_TYPE_DEBUFF = 3;
    private const int TOWER_TYPE_FIRE = 0;
    private const float DAMAGE_PER_FIRE_TOWER = 0.1f;

    private readonly RunicLighter _model = new();

    protected override RelicModel Model => this._model;

    public override void on_before_damage(Variant context)
    {
        GodotObject contextObj = context.AsGodotObject();
        GodotObject attack = contextObj?.Get("attack").AsGodotObject();
        GodotObject source = attack?.Get("source").AsGodotObject();
        if (source == null)
        {
            return;
        }

        int sourceType = (int)source.Get("type");
        string sourceTypeId = source.Get("type_id").AsString();
        if (sourceType != SOURCE_TYPE_DEBUFF || sourceTypeId != "burn_debuff")
        {
            return;
        }

        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject towersManager = runContext?.Get("towers_manager").AsGodotObject();
        if (towersManager == null)
        {
            return;
        }

        int fireTowers = (int)towersManager.Call("get_tower_count", TOWER_TYPE_FIRE);
        float extraMult = 1f + (DAMAGE_PER_FIRE_TOWER * fireTowers);
        float current = (float)contextObj.Get("extra_multiplicative");
        contextObj.Set("extra_multiplicative", current + extraMult);
    }
}
