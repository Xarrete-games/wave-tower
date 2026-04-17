using Godot;

[GlobalClass]
public partial class EventData : Resource
{
    public enum Type
    {
        OPTIONS,
        SHOP,
        CHOOSE_RELIC,
    }

    public enum Role
    {
        FRIENDLY,
        RANDOM,
        HOSTILE,
    }

    [Export]
    public string id { get; set; } = string.Empty;

    [Export]
    public Type type { get; set; }

    [Export]
    public Role role { get; set; }

    [Export]
    public string title { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string description { get; set; } = string.Empty;

    [Export]
    public Texture2D icon { get; set; }

    [Export]
    public Texture2D texture_background { get; set; }

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public string Id
    {
        get => id;
        set => id = value;
    }

    public Type EventType
    {
        get => type;
        set => type = value;
    }

    public Role EventRole
    {
        get => role;
        set => role = value;
    }

    public string Title
    {
        get => title;
        set => title = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public Texture2D Icon
    {
        get => icon;
        set => icon = value;
    }

    public Texture2D TextureBackground
    {
        get => texture_background;
        set => texture_background = value;
    }

    public Script RuntimeScript
    {
        get => runtime_script;
        set => runtime_script = value;
    }
}
