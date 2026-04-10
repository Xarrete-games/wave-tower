using Godot;

public partial class ConfigButton : Control
{
    private void _on_pressed()
    {
        Node audioManager = GetNode<Node>("/root/AudioManager");
        audioManager.Call("play_button_click");
        ClickEventsBus.EmitConfigButtonPressed();
    }

    private void _on_mouse_entered()
    {
        Node audioManager = GetNode<Node>("/root/AudioManager");
        audioManager.Call("play_button_hover");
    }
}