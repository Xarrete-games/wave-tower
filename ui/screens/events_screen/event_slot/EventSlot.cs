using Godot;
using System;

public partial class EventSlot : VBoxContainer
{
    public event Action<EventData> EventPressed;

    [Export]
    public TextureRect EventTexture;

    [Export]
    public RichTextLabel DescriptionLabel;

    [Export]
    public Label TitleLable;

    private EventData _event;

    public void SetEvent(EventData eventData)
    {
        if (eventData == null)
        {
            return;
        }

        EventTexture.Texture = eventData.Icon;
        DescriptionLabel.Text = eventData.Description;
        TitleLable.Text = eventData.Title;
        _event = eventData;
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        EventPressed?.Invoke(_event);
        GetNode<AudioManager>("/root/AudioManager").PlayButtonClick();
    }

    private void OnMouseEntered()
    {
        EventTexture.CustomMinimumSize = new Vector2(150, 150);
        GetNode<AudioManager>("/root/AudioManager").PlayButtonHover();
    }

    private void OnMouseExited()
    {
        EventTexture.CustomMinimumSize = new Vector2(100, 100);
    }
}
