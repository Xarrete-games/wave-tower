using Godot;
using System;

public partial class EventSlot : VBoxContainer
{
    public event Action<EventData> event_pressed;

    [Export]
    public TextureRect event_texture;

    [Export]
    public RichTextLabel description_label;

    [Export]
    public Label title_lable;

    private EventData _event;

    public void set_event(EventData eventData)
    {
        if (eventData == null)
        {
            return;
        }

        event_texture.Texture = eventData.icon;
        description_label.Text = eventData.description;
        title_lable.Text = eventData.title;
        _event = eventData;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        event_pressed?.Invoke(_event);
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
    }

    private void OnMouseEntered()
    {
        event_texture.CustomMinimumSize = new Vector2(150, 150);
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void OnMouseExited()
    {
        event_texture.CustomMinimumSize = new Vector2(100, 100);
    }
}
