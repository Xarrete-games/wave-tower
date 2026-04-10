using Godot;

[GlobalClass]
public partial class FoundationBreaker : ConsumableTargeteable
{
    public override void action(Variant p_target)
    {
        GodotObject tileMap = p_target.AsGodotObject();
        tileMap?.Call("unblock_tile_at_mouse");
    }
}
