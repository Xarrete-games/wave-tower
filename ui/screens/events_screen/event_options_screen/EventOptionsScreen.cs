using Godot;
using System;
using System.Collections.Generic;

public partial class EventOptionsScreen : Control
{
    public event Action event_completed;

    [Export]
    public PackedScene button_option_scene;

    [Export]
    public Control buttons_container;

    [Export]
    public TextureRect texture_rect;

    [Export]
    public Label title_label;

    [Export]
    public Label description_label;

    private EventScript _eventScriptInstance;

    public void set_event(EventData eventData)
    {
        if (eventData == null)
        {
            return;
        }

        title_label.Text = eventData.title;
        description_label.Text = eventData.description;
        texture_rect.Texture = eventData.TextureBackground;

        foreach (Node child in buttons_container.GetChildren())
        {
            child.QueueFree();
        }

        Script runtimeScript = eventData.RuntimeScript;
        if (runtimeScript == null)
        {
            GD.PushError($"Event data {eventData.id} has no runtime script assigned.");
            return;
        }

        if (runtimeScript is not CSharpScript csharpScript)
        {
            GD.PushError($"Runtime script for event {eventData.id} is not C#.");
            return;
        }

        _eventScriptInstance = csharpScript.New().Obj as EventScript;
        if (_eventScriptInstance == null)
        {
            GD.PushError($"Could not instantiate runtime script for event {eventData.id}.");
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

            EventOptionButton buttonOption = button_option_scene.Instantiate<EventOptionButton>();
            buttons_container.AddChild(buttonOption);
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
