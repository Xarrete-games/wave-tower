public class EventOptionData {
    public string Text {
        get;
        set;
    }
    = string.Empty;

    public EventOptionValue Data {
        get;
        set;
    }
    = EventOptionValue.Empty;

    public bool Disabled {
        get;
        set;
    }

    public EventOptionData() {
    }

    public EventOptionData(string optionText, EventOptionValue optionData, bool isDisabled = false) {
        Text = optionText;
        Data = optionData ?? EventOptionValue.Empty;
        Disabled = isDisabled;
    }
}

