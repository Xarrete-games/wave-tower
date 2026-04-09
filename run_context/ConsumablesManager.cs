using Godot;

[GlobalClass]
public partial class ConsumablesManager : RefCounted
{
    [Signal]
    public delegate void consumables_changeEventHandler(Godot.Collections.Array<Variant> consumables);

    [Signal]
    public delegate void consumable_addedEventHandler(Variant consumable);

    [Signal]
    public delegate void consumable_usedEventHandler(Variant consumable);

    [Signal]
    public delegate void consumable_clickedEventHandler(Variant consumable);

    private static readonly Script _hooksScript = GD.Load<Script>("res://core/hooks.gd");
    private readonly Godot.Collections.Array<Variant> _consumables = new();

    public bool is_full()
    {
        return this._consumables.Count == 5;
    }

    public void add_consumable(Variant consumable)
    {
        if (this.is_full())
        {
            return;
        }

        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj == null)
        {
            return;
        }

        this._consumables.Add(consumable);
        consumableObj.Connect("clicked", Callable.From<Variant>(this._on_consumable_clicked));
        consumableObj.Connect("used", Callable.From<Variant>(this._on_consumable_used));
        this.EmitSignal(SignalName.consumables_change, this._consumables);
        this.EmitSignal(SignalName.consumable_added, consumable);
    }

    public void _on_consumable_used(Variant consumable)
    {
        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj != null)
        {
            consumableObj.Disconnect("clicked", Callable.From<Variant>(this._on_consumable_clicked));
            consumableObj.Disconnect("used", Callable.From<Variant>(this._on_consumable_used));
        }

        this.EmitSignal(SignalName.consumable_used, consumable);
        this._consumables.Remove(consumable);
        this.EmitSignal(SignalName.consumables_change, this._consumables);
    }

    public void _on_consumable_clicked(Variant consumable)
    {
        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj == null)
        {
            return;
        }

        if (consumableObj.IsClass("ConsumableUsable"))
        {
            consumableObj.Call("use");
            _hooksScript.Call("on_consumable_used", consumable);
            consumableObj.EmitSignal("used", consumable);
        }
        else
        {
            this.EmitSignal(SignalName.consumable_clicked, consumable);
        }
    }
}
