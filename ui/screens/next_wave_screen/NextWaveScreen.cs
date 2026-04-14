using Godot;

public partial class NextWaveScreen : Control
{
    private void _on_next_wave_button_pressed()
    {
        ClickEvents.NextWavePressed?.Invoke();
        QueueFree();
    }
}
