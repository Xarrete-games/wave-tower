using Godot;

[GlobalClass]
public partial class BaseData : Resource
{
    public enum Rarity
    {
        COMMON,
        RARE,
        EPIC,
    }

    [Export]
    public string id { get; set; } = string.Empty;

    [Export]
    public string display_name { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string description { get; set; } = string.Empty;

    [Export]
    public Texture2D icon { get; set; }

    [Export]
    public Rarity rarity { get; set; } = Rarity.COMMON;

    // Keep exported snake_case for .tres/.tscn compatibility and expose PascalCase aliases for C# code.
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

    public Rarity DataRarity
    {
        get => rarity;
        set => rarity = value;
    }

    public virtual Variant create_item()
    {
        return default;
    }

    public virtual Variant CreateItem()
    {
        return create_item();
    }
}
