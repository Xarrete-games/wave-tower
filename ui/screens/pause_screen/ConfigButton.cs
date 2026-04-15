using Godot;

public partial class ConfigButton : Control
{
    private void _on_pressed()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.play_button_click();
        ClickEvents.ConfigButtonPressed?.Invoke();
    }

    private void _on_mouse_entered()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.play_button_hover();
    }
}
