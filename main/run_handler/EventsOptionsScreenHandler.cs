using Godot;
using System.Threading.Tasks;

public partial class EventsOptionsScreenHandler : Node
{
    [Export]
    public PackedScene events_options_screen;

    public async Task ShowOptionsEventAsync(Variant eventData, CanvasLayer eventLayer)
    {
        Node optionsScreen = this.events_options_screen.Instantiate();

        if (!IsInsideTree())
        {
            await ToSignal(this, Node.SignalName.Ready);
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        eventLayer.AddChild(optionsScreen);
        optionsScreen.Call("set_event", eventData);
        await ToSignal(optionsScreen, "event_completed");
    }
}