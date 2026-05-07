using Godot;

public partial class DeathScreen : CanvasLayer
{
    public override void _Ready()
    {
        AudioManager audioManager = GetNode<AudioManager>("/root/AudioManager");
        audioManager.PlayDefeatedSound();
        GetTree().Paused = true;
    }

    private void OnTryAgainButtonXarretaPressed()
    {
        GetTree().Paused = false;
        ClickEvents.ResetGameButtonPressed?.Invoke();
        QueueFree();
    }
}
