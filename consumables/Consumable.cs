using Godot;

[GlobalClass]
public partial class Consumable : RefCounted
{
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

    public ConsumableData Data => this.data.As<ConsumableData>();

    public Consumable()
    {
    }

    public Consumable(ConsumableData p_data)
    {
        this.init(p_data);
    }

    public virtual void init(ConsumableData p_data)
    {
        this.data = p_data;
    }

    public virtual bool requires_target()
    {
        return false;
    }

    public Variant get_source()
    {
        string id = this.Data?.id ?? string.Empty;
        return new Source(Source.SourceType.CONSUMABLE, id, this);
    }

    protected Node GetSingleton(string name)
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
    }
}
