using Godot;
using System.Threading.Tasks;

public partial class EventsOptionsScreenHandler : Node
{
    [Export]
    public PackedScene events_options_screen;

    public async Task ShowOptionsEventAsync(EventData eventData, CanvasLayer eventLayer)
    {
        EventOptionsScreen optionsScreen = events_options_screen.Instantiate<EventOptionsScreen>();

        if (!IsInsideTree())
        {
            await ToSignal(this, Node.SignalName.Ready);
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        eventLayer.AddChild(optionsScreen);
        optionsScreen.set_event(eventData);

        var completion = new TaskCompletionSource<bool>();
        void OnCompleted()
        {
            completion.TrySetResult(true);
        }

        optionsScreen.event_completed += OnCompleted;
        try
        {
            await completion.Task;
        }
        finally
        {
            if (GodotObject.IsInstanceValid(optionsScreen))
            {
                optionsScreen.event_completed -= OnCompleted;
            }
        }
    }
}