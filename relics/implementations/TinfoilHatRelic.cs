using Godot;

[GlobalClass]
public partial class TinfoilHatRelic : RelicRuntimeAdapter
{
    private readonly TinfoilHat _model = new();

    protected override RelicModel Model => this._model;

    public override void on_relic_added(Variant relicAdded)
    {
        if (this.disabled)
        {
            return;
        }

        GodotObject relicObj = relicAdded.AsGodotObject();
        GodotObject data = relicObj?.Get("data").AsGodotObject();
        if (data == null)
        {
            return;
        }

        bool isCursed = (bool)data.Get("is_cursed");
        if (!isCursed)
        {
            return;
        }

        string relicId = data.Get("id").AsString();
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject relicsManager = runContext?.Get("relics_manager").AsGodotObject();
        relicsManager?.Call("remove_relic", relicId);
        this.disabled = true;
    }
}
