using Godot;
using System;

public partial class EventOptionButton : Button
{
    public event Action<object> OptionSelected;

    public object OptionData;

    public void DisableOption()
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
        OptionSelected?.Invoke(OptionData);
    }
}
