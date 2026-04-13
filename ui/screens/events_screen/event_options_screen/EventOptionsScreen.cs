using Godot;
using System.Collections.Generic;

public partial class EventOptionsScreen : Control
{
    [Signal]
    public delegate void event_completedEventHandler();

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

    public void set_event(Variant eventData)
    {
        GodotObject eventDataObj = eventData.AsGodotObject();
        if (eventDataObj == null)
        {
            return;
        }

        this.title_label.Text = (string)eventDataObj.Get("title");
        this.description_label.Text = (string)eventDataObj.Get("description");
        this.texture_rect.Texture = eventDataObj.Get("texture_background").As<Texture2D>();

        foreach (Node child in this.buttons_container.GetChildren())
        {
            child.QueueFree();
        }

        Variant runtimeScriptVariant = eventDataObj.Get("runtime_script");
        Script runtimeScript = runtimeScriptVariant.As<Script>();
        if (runtimeScript == null)
        {
            GD.PushError($"Event data {eventDataObj.Get("id")} has no runtime script assigned.");
            return;
        }

        if (runtimeScript is CSharpScript csharpScript)
        {
            this._eventScriptInstance = csharpScript.New().Obj as EventScript;
        }
        else
        {
            this._eventScriptInstance = runtimeScript.Call("new").Obj as EventScript;
        }
        if (this._eventScriptInstance == null)
        {
            GD.PushError($"Could not instantiate runtime script for event {eventDataObj.Get("id")}.");
            return;
        }

        List<EventOptionData> options = this._eventScriptInstance.get_options();
        int index = 0;
        foreach (EventOptionData optionData in options)
        {
            if (optionData == null)
            {
                continue;
            }

            EventOptionButton buttonOption = this.button_option_scene.Instantiate<EventOptionButton>();
            this.buttons_container.AddChild(buttonOption);
            buttonOption.Text = optionData.text;
            buttonOption.option_data = optionData.data;
            buttonOption.Name = $"OptionButton_{index}";
            index += 1;

            if (optionData.disabled)
            {
                buttonOption.disable_option();
            }

            buttonOption.option_selected += this.OnOptionSelected;
        }
    }

    private void OnOptionSelected(Variant data)
    {
        this._eventScriptInstance?.handle_response(data);
        EmitSignal(SignalName.event_completed);
        QueueFree();
    }
}