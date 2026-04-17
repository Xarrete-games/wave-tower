using Godot;
using System;

public partial class EventOptionButton : Button
{
    public event Action<object> option_selected;

    public object option_data;

    public void disable_option()
    {
        Disabled = true;
        Modulate = new Color(0.5f, 0.5f, 0.5f);
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void OnPressed()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        option_selected?.Invoke(option_data);
    }
}
