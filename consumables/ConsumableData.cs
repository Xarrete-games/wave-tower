using Godot;

[GlobalClass]
public partial class ConsumableData : BaseData
{
    [ExportGroup("Consumable")]
    [Export]
    public int consumable_type { get; set; }

    [Export]
    public int targeting_type { get; set; }

    [Export]
    public Texture2D cursor_icon { get; set; }

    [Export]
    public Texture2D cursor_icon_used { get; set; }

    [Export]
    public AudioStream use_sound { get; set; }

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public override Variant create_item()
    {
        if (this.runtime_script is GDScript gdscript)
        {
            return gdscript.Call("new", this);
        }

        GD.PushError($"[ConsumableData] Missing or invalid runtime_script for '{this.id}'");
        return default;
    }
}
