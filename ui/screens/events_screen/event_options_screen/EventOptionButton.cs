using Godot;
using System;

public partial class EventOptionButton : Button
{
    public event Action<object> option_selected;

    public object option_data;

    public void disable_option()
    {
        this.Disabled = true;
        this.Modulate = new Color(0.5f, 0.5f, 0.5f);
    }

    private void _on_mouse_entered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void _on_pressed()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        this.option_selected?.Invoke(this.option_data);
    }
}
