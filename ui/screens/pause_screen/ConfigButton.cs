using Godot;

public partial class ConfigButton : Control
{
    private void OnPressed()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.play_button_click();
        ClickEvents.ConfigButtonPressed?.Invoke();
    }

    private void OnMouseEntered()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.play_button_hover();
    }
}
