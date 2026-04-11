using Godot;

public partial class EventOptionButton : Button
{
    [Signal]
    public delegate void option_selectedEventHandler(Variant data);

    [Export]
    public Variant option_data;

    public void disable_option()
    {
        this.Disabled = true;
        this.Modulate = new Color(0.5f, 0.5f, 0.5f);
    }

    private void _on_mouse_entered()
    {
        GetNode<Node>("/root/AudioManager").Call("play_button_hover");
    }

    private void _on_pressed()
    {
        GetNode<Node>("/root/AudioManager").Call("play_button_click");
        EmitSignal(SignalName.option_selected, this.option_data);
    }
}
