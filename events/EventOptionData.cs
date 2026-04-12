using Godot;

public partial class EventOptionData : RefCounted
{
    public string text { get; set; } = string.Empty;
    public Variant data { get; set; }
    public bool disabled { get; set; }

    public EventOptionData() { }

    public EventOptionData(string p_text, Variant p_data, bool p_disabled = false)
    {
        this.text = p_text;
        this.data = p_data;
        this.disabled = p_disabled;
    }
}
