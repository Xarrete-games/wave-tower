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

    public string Id
    {
        get => id;
        set => id = value;
    }

    public string DisplayName
    {
        get => display_name;
        set => display_name = value;
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

    public int RarityValue
    {
        get => rarity;
        set => rarity = value;
    }

    public bool ShowCounter
    {
        get => show_counter;
        set => show_counter = value;
    }

    public int HealthPrice
    {
        get => health_price;
        set => health_price = value;
    }

    public bool IsCursed
    {
        get => is_cursed;
        set => is_cursed = value;
    }

    public bool IsTome
    {
        get => is_tome;
        set => is_tome = value;
    }

    public bool OnlyForEvents
    {
        get => only_for_events;
        set => only_for_events = value;
    }

    public int MaxStacks
    {
        get => max_stacks;
        set => max_stacks = value;
    }

    public Script RuntimeScript
    {
        get => runtime_script;
        set => runtime_script = value;
    }

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

    public Relic CreateItem()
    {
        return create_item();
    }
}
