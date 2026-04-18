using Godot;

[GlobalClass]
public partial class RelicData : Resource
{
    [Export]
    public string Id { get; set; } = string.Empty;

    [Export]
    public string DisplayName { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public int Rarity { get; set; }

    [ExportGroup("Relic")]
    [Export]
    public bool ShowCounter { get; set; }

    [Export]
    public int HealthPrice { get; set; }

    [Export]
    public bool IsCursed { get; set; }

    [Export]
    public bool IsTome { get; set; }

    [Export]
    public bool OnlyForEvents { get; set; }

    [Export]
    public int MaxStacks { get; set; } = 1;

    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public Relic CreateItem()
    {
        Relic relic = RelicModelFactory.CreateById(Id);
        if (relic == null)
        {
            GD.PushError($"[RelicData] Could not create Relic instance for id '{Id}'");
            return null;
        }

        relic.SetupData(this);
        return relic;
    }
}
