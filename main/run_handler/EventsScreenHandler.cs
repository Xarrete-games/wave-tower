using Godot;
using System;
using System.Threading.Tasks;

public partial class EventsScreenHandler : Node
{
    public event Action event_finished;

    private ShopScreenHandler _shopScreenHandler;
    private EventsOptionsScreenHandler _eventsOptionsScreenHandler;
    private ChooseRelicScreenHandler _chooseRelicScreenHandler;

    public override void _Ready()
    {
        _shopScreenHandler = GetNode<ShopScreenHandler>("ShopScreenHandler");
        _eventsOptionsScreenHandler = GetNode<EventsOptionsScreenHandler>("EventsOptionsScreenHandler");
        _chooseRelicScreenHandler = GetNode<ChooseRelicScreenHandler>("ChooseRelicScreenHandler");
    }

    public async Task ShowEventSelectedAsync(EventData eventData, CanvasLayer eventLayer)
    {
        if (eventData == null)
        {
            event_finished?.Invoke();
            return;
        }

        int eventType = (int)eventData.type;
        if (eventType == 2)
        {
            await _chooseRelicScreenHandler.ShowChooseRelicEventAsync(eventLayer);
        }
        else if (eventType == 1)
        {
            await _shopScreenHandler.OpenShopAsync(eventLayer);
        }
        else if (eventType == 0)
        {
            await _eventsOptionsScreenHandler.ShowOptionsEventAsync(eventData, eventLayer);
        }

        event_finished?.Invoke();
    }
}