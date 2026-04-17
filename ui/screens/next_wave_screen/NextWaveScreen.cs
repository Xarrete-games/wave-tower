using Godot;

public partial class NextWaveScreen : Control
{
    private void OnNextWaveButtonPressed()
    {
        ClickEvents.NextWavePressed?.Invoke();
        QueueFree();
    }
}
