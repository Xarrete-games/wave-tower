using Godot;
using System;
using System.Collections.Generic;

public partial class EventScreen : Control
{
    public event Action<EventData> EventSelected;

    private static readonly PackedScene EventSlotScene = GD.Load<PackedScene>("uid://cl23rwbjcvak5");

    [Export]
    public Control EventsContainer;

    public void SetEvents(IReadOnlyList<EventData> eventsData)
    {
        for (int index = 0; index < eventsData.Count; index++)
        {
            EventSlot eventSlot = EventSlotScene.Instantiate<EventSlot>();
            EventsContainer.AddChild(eventSlot);
            eventSlot.SetEvent(eventsData[index]);
            eventSlot.EventPressed += OnEventPressed;
        }
    }

    private void OnEventPressed(EventData @event)
    {
        EventSelected?.Invoke(@event);
        QueueFree();
    }
}
