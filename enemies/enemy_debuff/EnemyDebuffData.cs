using Godot;

[GlobalClass]
public partial class EnemyDebuffData : BaseData
{
    [ExportGroup("Debuff")]
    [Export]
    public int debuff_type { get; set; }

    [Export]
    public float value { get; set; }

    [Export]
    public float duration { get; set; }

    [Export]
    public float tick_interval { get; set; }

    [Export]
    public int max_stacks { get; set; } = 99;

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public override Variant create_item()
    {
        if (this.runtime_script is GDScript gdscript)
        {
            return gdscript.Call("new", this);
        }

        GD.PushError($"[EnemyDebuffData] Missing or invalid runtime_script for '{this.id}'");
        return default;
    }
}
