using Godot;
using System.Threading.Tasks;

public partial class EventsOptionsScreenHandler : Node
{
    [Export]
    public PackedScene EventsOptionsScreen;

    public async Task ShowOptionsEventAsync(EventData eventData, CanvasLayer eventLayer)
    {
        EventOptionsScreen optionsScreen = EventsOptionsScreen.Instantiate<EventOptionsScreen>();

        if (!IsInsideTree())
        {
            await ToSignal(this, Node.SignalName.Ready);
        }

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        eventLayer.AddChild(optionsScreen);
        optionsScreen.SetEvent(eventData);

        var completion = new TaskCompletionSource<bool>();
        void OnCompleted()
        {
            completion.TrySetResult(true);
        }

        optionsScreen.EventCompleted += OnCompleted;
        try
        {
            await completion.Task;
        }
        finally
        {
            if (IsInstanceValid(optionsScreen))
            {
                optionsScreen.EventCompleted -= OnCompleted;
            }
        }
    }
}