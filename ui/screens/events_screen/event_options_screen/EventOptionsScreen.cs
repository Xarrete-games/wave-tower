using Godot;
using System;
using System.Collections.Generic;

public partial class EventOptionsScreen : Control
{
    public event Action EventCompleted;

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

    public void SetEvent(EventData eventData)
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

        List<EventOptionData> options = _eventScriptInstance.GetOptions();
        int index = 0;
        foreach (EventOptionData optionData in options)
        {
            if (optionData == null)
            {
                continue;
            }

            EventOptionButton buttonOption = ButtonOptionScene.Instantiate<EventOptionButton>();
            ButtonsContainer.AddChild(buttonOption);
            buttonOption.Text = optionData.Text;
            buttonOption.OptionData = optionData.Data;
            buttonOption.Name = $"OptionButton_{index}";
            index += 1;

            if (optionData.Disabled)
            {
                buttonOption.DisableOption();
            }

            buttonOption.OptionSelected += OnOptionSelected;
        }
    }

    private void OnOptionSelected(object data)
    {
        _eventScriptInstance?.HandleResponse(data);
        EventCompleted?.Invoke();
        QueueFree();
    }
}
