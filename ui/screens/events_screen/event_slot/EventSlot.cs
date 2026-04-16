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

        this.event_texture.Texture = eventData.icon;
        this.description_label.Text = eventData.description;
        this.title_lable.Text = eventData.title;
        this._event = eventData;
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        this.event_pressed?.Invoke(this._event);
        GetNode<AudioManager>("/root/AudioManager").play_button_click();
    }

    private void _on_mouse_entered()
    {
        this.event_texture.CustomMinimumSize = new Vector2(150, 150);
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
    }

    private void _on_mouse_exited()
    {
        this.event_texture.CustomMinimumSize = new Vector2(100, 100);
    }
}
