using Godot;

public partial class NextLevelScreen : Control
{
    private void _on_next_level_button_pressed()
    {
        ClickEventsBus.EmitNextLevelPressed();
        QueueFree();
    }
}