using Godot;

[GlobalClass]
public partial class BuffData : BaseData
{
    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public Variant create_item(Variant source = default, int value = 0)
    {
        if (this.runtime_script == null)
        {
            GD.PushError($"[BuffData] Missing runtime_script for buff id: {this.id}");
            return default;
        }

        if (!this.runtime_script.HasMethod("create_instance"))
        {
            GD.PushError($"[BuffData] runtime_script must implement static create_instance(data, source, value): {this.runtime_script.ResourcePath}");
            return default;
        }

        return this.runtime_script.Call("create_instance", this, source, value);
    }
}
