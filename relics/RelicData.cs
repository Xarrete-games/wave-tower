using Godot;

[GlobalClass]
public partial class RelicData : Resource
{
    [Export]
    public string id { get; set; } = string.Empty;

    [Export]
    public string display_name { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string description { get; set; } = string.Empty;

    [Export]
    public Texture2D icon { get; set; }

    [Export]
    public int rarity { get; set; }

    [ExportGroup("Relic")]
    [Export]
    public bool show_counter { get; set; }

    [Export]
    public int health_price { get; set; }

    [Export]
    public bool is_cursed { get; set; }

    [Export]
    public bool is_tome { get; set; }

    [Export]
    public bool only_for_events { get; set; }

    [Export]
    public int max_stacks { get; set; } = 1;

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public Relic create_item()
    {
        Relic relic = RelicModelFactory.CreateById(id);
        if (relic == null)
        {
            GD.PushError($"[RelicData] Could not create Relic instance for id '{id}'");
            return null;
        }

        relic.SetupData(this);
        return relic;
    }
}
