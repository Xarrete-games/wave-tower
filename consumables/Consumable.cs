using Godot;
using System;

public class Consumable
{
    public event Action<Consumable> used;
    public event Action<Consumable> clicked;

    public enum Type
    {
        OTHER,
        POTION,
    }

    public ConsumableData data { get; set; }

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

    public Source get_source()
    {
        string id = this.data?.id ?? string.Empty;
        return new Source(Source.SourceType.CONSUMABLE, id);
    }

    public void emit_used()
    {
        this.used?.Invoke(this);
    }

    public void emit_clicked()
    {
        this.clicked?.Invoke(this);
    }

    protected Node GetSingleton(string name)
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>($"/root/{name}");
    }
}
