using Godot;

[GlobalClass]
public partial class EventData : Resource
{
    public enum EventType
    {
        OPTIONS,
        SHOP,
        CHOOSE_RELIC,
    }

    public enum EventRole
    {
        FRIENDLY,
        RANDOM,
        HOSTILE,
    }

    [Export]
    public string Id { get; set; } = string.Empty;

    [Export]
    public EventType Type { get; set; }

    [Export]
    public EventRole Role { get; set; }

    [Export]
    public string Title { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Export]
    public Texture2D Icon { get; set; }

    // Legacy compatibility aliases (non-exported).
    public string id
    {
        get => Id;
        set => Id = value;
    }

    public string description
    {
        get => Description;
        set => Description = value;
    }

    public Texture2D icon
    {
        get => Icon;
        set => Icon = value;
    }

    public EventType type
    {
        get => Type;
        set => Type = value;
    }

    public EventRole role
    {
        get => Role;
        set => Role = value;
    }

    public string title
    {
        get => Title;
        set => Title = value;
    }

    [Export]
    public Texture2D TextureBackground { get; set; }

    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public EventType EventTypeValue
    {
        get => Type;
        set => Type = value;
    }

    public EventRole EventRoleValue
    {
        get => Role;
        set => Role = value;
    }

}
