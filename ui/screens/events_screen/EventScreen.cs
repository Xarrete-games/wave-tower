using Godot;

public partial class EventScreen : Control
{
    [Signal]
    public delegate void event_selectedEventHandler(Variant @event);

    private static readonly PackedScene EventSlotScene = GD.Load<PackedScene>("uid://cl23rwbjcvak5");

    [Export]
    public Control events_container;

    public void set_events(Godot.Collections.Array<Variant> events)
    {
        for (int index = 0; index < events.Count; index++)
        {
            EventSlot eventSlot = EventSlotScene.Instantiate<EventSlot>();
            this.events_container.AddChild(eventSlot);
            eventSlot.set_event(events[index]);
            eventSlot.event_pressed += this._on_event_pressed;
        }
    }

    private void _on_event_pressed(Variant @event)
    {
        EmitSignal(SignalName.event_selected, @event);
        QueueFree();
    }
}
