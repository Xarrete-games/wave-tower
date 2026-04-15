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
        this._shopScreenHandler = GetNode<ShopScreenHandler>("ShopScreenHandler");
        this._eventsOptionsScreenHandler = GetNode<EventsOptionsScreenHandler>("EventsOptionsScreenHandler");
        this._chooseRelicScreenHandler = GetNode<ChooseRelicScreenHandler>("ChooseRelicScreenHandler");
    }

    public async Task ShowEventSelectedAsync(EventData eventData, CanvasLayer eventLayer)
    {
        if (eventData == null)
        {
            this.event_finished?.Invoke();
            return;
        }

        int eventType = (int)eventData.type;
        if (eventType == 2)
        {
            await this._chooseRelicScreenHandler.ShowChooseRelicEventAsync(eventLayer);
        }
        else if (eventType == 1)
        {
            await this._shopScreenHandler.OpenShopAsync(eventLayer);
        }
        else if (eventType == 0)
        {
            await this._eventsOptionsScreenHandler.ShowOptionsEventAsync(eventData, eventLayer);
        }

        this.event_finished?.Invoke();
    }
}