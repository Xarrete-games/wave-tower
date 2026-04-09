using Godot;

[GlobalClass]
public partial class RelicData : Resource
{
    [Export]
    public string id { get; set; } = string.Empty;

    [Export]
    public string display_name { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string description { get; set; } = string.Empty;

    [Export]
    public Texture2D icon { get; set; }

    [Export]
    public int rarity { get; set; }

    [ExportGroup("Relic")]
    [Export]
    public bool show_counter { get; set; }

    [Export]
    public int health_price { get; set; }

    [Export]
    public bool is_cursed { get; set; }

    [Export]
    public bool is_tome { get; set; }

    [Export]
    public bool only_for_events { get; set; }

    [Export]
    public int max_stacks { get; set; } = 1;

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public Variant create_item()
    {
        GodotObject item = this.CreateFromRuntimeScript();
        if (item == null)
        {
            GD.PushError($"[RelicData] Could not instantiate runtime_script for relic '{this.id}'");
            return default;
        }

        item.Set("data", this);
        return item;
    }

    private GodotObject CreateFromRuntimeScript()
    {
        if (this.runtime_script == null)
        {
            return null;
        }

        if (this.runtime_script is GDScript gdscript)
        {
            Variant created = gdscript.Call("new", this);
            return created.VariantType == Variant.Type.Nil ? null : created.AsGodotObject();
        }

        if (this.runtime_script is CSharpScript csharpScript)
        {
            Variant created = csharpScript.New();
            return created.VariantType == Variant.Type.Nil ? null : created.AsGodotObject();
        }

        return null;
    }
}
