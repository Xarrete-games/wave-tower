using Godot;

public partial class EventSlot : VBoxContainer
{
    [Signal]
    public delegate void event_pressedEventHandler(Variant @event);

    [Export]
    public TextureRect event_texture;

    [Export]
    public RichTextLabel description_label;

    [Export]
    public Label title_lable;

    private Variant _event;

    public void set_event(Variant @event)
    {
        GodotObject eventObj = @event.AsGodotObject();
        if (eventObj == null)
        {
            return;
        }

        this.event_texture.Texture = eventObj.Get("icon").As<Texture2D>();
        this.description_label.Text = eventObj.Get("description").AsString();
        this.title_lable.Text = eventObj.Get("title").AsString();
        this._event = @event;
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        EmitSignal(SignalName.event_pressed, this._event);
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
