using Godot;
using System.IO;

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

    public EventScript CreateRuntimeEventScript()
    {
        string scriptName = Path.GetFileNameWithoutExtension(RuntimeScript?.ResourcePath)?.ToLowerInvariant() ?? string.Empty;
        EventScript eventScript = scriptName switch
        {
            "bloodpactscript" => new BloodPactScript(),
            "budatemplescript" => new BudaTempleScript(),
            "chesteventscript" => new ChestEventScript(),
            "fountainsofwishesscript" => new FountainsOfWishesScript(),
            "highwayrobberyscript" => new HighwayRobberyScript(),
            "libraryeventscript" => new LibraryEventScript(),
            "potionseventscript" => new PotionsEventScript(),
            "sanctuaryeventscript" => new SanctuaryEventScript(),
            _ => null,
        };

        if (eventScript == null)
        {
            GD.PushError($"[EventData] Unknown RuntimeScript '{RuntimeScript?.ResourcePath}' for event '{Id}'");
        }

        return eventScript;
    }

}
