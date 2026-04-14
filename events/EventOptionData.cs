public class EventOptionData
{
    public string text { get; set; } = string.Empty;
    public object data { get; set; }
    public bool disabled { get; set; }

    public EventOptionData() { }

    public EventOptionData(string p_text, object p_data, bool p_disabled = false)
    {
        this.text = p_text;
        this.data = p_data;
        this.disabled = p_disabled;
    }
}
