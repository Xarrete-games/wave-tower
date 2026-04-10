using Godot;

public partial class DeathScreen : CanvasLayer
{
    public override void _Ready()
    {
        Node audioManager = GetNode<Node>("/root/AudioManager");
        audioManager.Call("play_defeated_sound");
        GetTree().Paused = true;
    }

    private void _on_try_again_button_xarreta_pressed()
    {
        GetTree().Paused = false;
        ClickEventsBus.EmitResetGameButtonPressed();
        QueueFree();
    }
}