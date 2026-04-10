using Godot;

[GlobalClass]
public partial class Consumable : RefCounted
{
    private const int CONSUMABLE_SOURCE_TYPE = 2;
    private static readonly Script _sourceScript = GD.Load<Script>("res://core/source.gd");

    [Signal]
    public delegate void usedEventHandler(Variant consumable);

    [Signal]
    public delegate void clickedEventHandler(Variant consumable);

    public enum Type
    {
        OTHER,
        POTION,
    }

    public Variant data { get; set; }

    public virtual bool requires_target()
    {
        return false;
    }

    public Variant get_source()
    {
        GodotObject dataObj = this.data.AsGodotObject();
        string id = dataObj == null ? string.Empty : dataObj.Get("id").AsString();
        return _sourceScript.Call("new", CONSUMABLE_SOURCE_TYPE, id, this);
    }

    protected Node GetSingleton(string name)
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
    }
}
