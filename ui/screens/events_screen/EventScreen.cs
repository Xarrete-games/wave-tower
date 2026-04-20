using Godot;
using System;

public partial class EventScreen : Control
{
    public event Action<EventData> EventSelected;

    private static readonly PackedScene EventSlotScene = GD.Load<PackedScene>("uid://cl23rwbjcvak5");

    [Export]
    public Control EventsContainer;

    public void SetEvents(Godot.Collections.Array<EventData> events)
    {
        for (int index = 0; index < events.Count; index++)
        {
            EventSlot eventSlot = EventSlotScene.Instantiate<EventSlot>();
            EventsContainer.AddChild(eventSlot);
            eventSlot.SetEvent(events[index]);
            eventSlot.EventPressed += OnEventPressed;
        }
    }

    private void OnEventPressed(EventData @event)
    {
        EventSelected?.Invoke(@event);
        QueueFree();
    }
}
