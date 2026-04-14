using Godot;

[GlobalClass]
public partial class BuffData : BaseData
{
    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public Variant create_item(Source source, int value = 0)
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
            if (buff == null || source == null)
            {
                GD.PushError($"[BuffData] Could not instantiate C# runtime buff for id: {this.id}");
                return default;
            }

            buff.source = source;
            buff.data = this;
            buff.value = value;
            return buff;
        }

        GD.PushError($"[BuffData] runtime_script must be C# script for buff id: {this.id}");
        return default;
    }
}
