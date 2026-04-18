using Godot;
using System;
using System.Collections.Generic;

public partial class EventOptionsScreen : Control
{
    public event Action event_completed;

    [Export]
    public PackedScene ButtonOptionScene;

    [Export]
    public Control ButtonsContainer;

    [Export]
    public TextureRect TextureRect;

    [Export]
    public Label TitleLabel;

    [Export]
    public Label DescriptionLabel;

    private EventScript _eventScriptInstance;

    public void set_event(EventData eventData)
    {
        if (eventData == null)
        {
            return;
        }

        TitleLabel.Text = eventData.Title;
        DescriptionLabel.Text = eventData.Description;
        TextureRect.Texture = eventData.TextureBackground;

        foreach (Node child in ButtonsContainer.GetChildren())
        {
            child.QueueFree();
        }

        _eventScriptInstance = eventData.CreateRuntimeEventScript();
        if (_eventScriptInstance == null)
        {
            GD.PushError($"Could not instantiate runtime script for event {eventData.Id}.");
            return;
        }

        List<EventOptionData> options = _eventScriptInstance.get_options();
        int index = 0;
        foreach (EventOptionData optionData in options)
        {
            if (optionData == null)
            {
                continue;
            }

            EventOptionButton buttonOption = ButtonOptionScene.Instantiate<EventOptionButton>();
            ButtonsContainer.AddChild(buttonOption);
            buttonOption.Text = optionData.text;
            buttonOption.option_data = optionData.data;
            buttonOption.Name = $"OptionButton_{index}";
            index += 1;

            if (optionData.disabled)
            {
                buttonOption.disable_option();
            }

            buttonOption.option_selected += OnOptionSelected;
        }
    }

    private void OnOptionSelected(object data)
    {
        _eventScriptInstance?.handle_response(data);
        event_completed?.Invoke();
        QueueFree();
    }
}
