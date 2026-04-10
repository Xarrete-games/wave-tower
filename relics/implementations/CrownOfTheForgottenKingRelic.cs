using Godot;

[GlobalClass]
public partial class CrownOfTheForgottenKingRelic : RelicRuntimeAdapter
{
    private readonly CrownOfTheForgottenKing _model = new();

    protected override RelicModel Model => this._model;

    public override void on_wave_finished()
    {
        GodotObject runContext = (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
        GodotObject compositeTileMap = runContext?.Get("composite_tile_map").AsGodotObject();
        compositeTileMap?.Call("destroy_random_buildeable_tile");
    }
}
