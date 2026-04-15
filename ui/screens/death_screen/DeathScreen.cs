using Godot;

public partial class DeathScreen : CanvasLayer
{
    public override void _Ready()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.play_defeated_sound();
        GetTree().Paused = true;
    }

    private void _on_try_again_button_xarreta_pressed()
    {
        GetTree().Paused = false;
        ClickEvents.ResetGameButtonPressed?.Invoke();
        QueueFree();
    }
}
