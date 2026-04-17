public class EventOptionData {
    public string text {
        get;
        set;
    }
    = string.Empty;
    public object data {
        get;
        set;
    }
    public bool disabled {
        get;
        set;
    }
    public EventOptionData() {
    }
    public EventOptionData(string optionText, object optionData, bool isDisabled = false) {
        text = optionText;
        data = optionData;
        disabled = isDisabled;
    }
}

