using Godot;
using System;

public partial class EventScreen : Control
{
    public event Action<EventData> event_selected;

    private static readonly PackedScene EventSlotScene = GD.Load<PackedScene>("uid://cl23rwbjcvak5");

    [Export]
    public Control events_container;

    public void set_events(Godot.Collections.Array<EventData> events)
    {
        for (int index = 0; index < events.Count; index++)
        {
            EventSlot eventSlot = EventSlotScene.Instantiate<EventSlot>();
            events_container.AddChild(eventSlot);
            eventSlot.set_event(events[index]);
            eventSlot.event_pressed += OnEventPressed;
        }
    }

    private void OnEventPressed(EventData @event)
    {
        event_selected?.Invoke(@event);
        QueueFree();
    }
}
