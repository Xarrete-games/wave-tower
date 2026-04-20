public class EventOptionData {
    public string Text {
        get;
        set;
    }
    = string.Empty;
    public object Data {
        get;
        set;
    }
    public bool Disabled {
        get;
        set;
    }
    public EventOptionData() {
    }
    public EventOptionData(string optionText, object optionData, bool isDisabled = false) {
        Text = optionText;
        Data = optionData;
        Disabled = isDisabled;
    }
}

