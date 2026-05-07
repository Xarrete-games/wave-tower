using Godot;

public partial class NextLevelScreen : Control
{
    private void OnNextLevelButtonPressed()
    {
        ClickEvents.NextLevelPressed?.Invoke();
        QueueFree();
    }
}
