using Godot;

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

    private GodotObject _eventScriptInstance;

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

        this._eventScriptInstance = runtimeScript.Call("new").AsGodotObject();
        if (this._eventScriptInstance == null)
        {
            GD.PushError($"Could not instantiate runtime script for event {eventDataObj.Get("id")}.");
            return;
        }

        var options = this._eventScriptInstance.Call("get_options").AsGodotArray<Variant>();
        int index = 0;
        foreach (Variant optionDataVariant in options)
        {
            GodotObject optionData = optionDataVariant.AsGodotObject();
            if (optionData == null)
            {
                continue;
            }

            Node buttonOption = this.button_option_scene.Instantiate();
            this.buttons_container.AddChild(buttonOption);
            buttonOption.Set("text", optionData.Get("text"));
            buttonOption.Set("option_data", optionData.Get("data"));
            buttonOption.Name = $"OptionButton_{index}";
            index += 1;

            if ((bool)optionData.Get("disabled"))
            {
                buttonOption.Call("disable_option");
            }

            buttonOption.Connect("option_selected", Callable.From<Variant>(this.OnOptionSelected));
        }
    }

    private void OnOptionSelected(Variant data)
    {
        this._eventScriptInstance?.Call("handle_response", data);
        EmitSignal(SignalName.event_completed);
        QueueFree();
    }
}