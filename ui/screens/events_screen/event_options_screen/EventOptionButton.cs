using Godot;
using System;

public partial class EventOptionButton : Button
{
    public event Action<EventOptionValue> OptionSelected;

    public EventOptionValue OptionData { get; set; } = EventOptionValue.Empty;

    public void DisableOption()
    {
        Disabled = true;
        Modulate = new Color(0.5f, 0.5f, 0.5f);
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").PlayButtonHover();
    }

    private void OnPressed()
    {
        GetNode<AudioManager>("/root/AudioManager").PlayButtonClick();
        OptionSelected?.Invoke(OptionData);
    }
}
