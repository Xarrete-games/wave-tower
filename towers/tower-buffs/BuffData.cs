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

        if (this.runtime_script is CSharpScript csharpScript)
        {
            Variant created = csharpScript.New();
            TowerBuffStatsModifier buff = created.As<TowerBuffStatsModifier>();
            Source buffSource = source.As<Source>();
            if (buff == null || buffSource == null)
            {
                GD.PushError($"[BuffData] Could not instantiate C# runtime buff for id: {this.id}");
                return default;
            }

            buff.source = buffSource;
            buff.data = this;
            buff.value = value;
            return buff;
        }

        if (!this.runtime_script.HasMethod("create_instance"))
        {
            GD.PushError($"[BuffData] runtime_script must implement static create_instance(data, source, value): {this.runtime_script.ResourcePath}");
            return default;
        }

        return this.runtime_script.Call("create_instance", this, source, value);
    }
}
