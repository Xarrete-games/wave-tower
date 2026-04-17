using Godot;

[GlobalClass]
public partial class BaseData : Resource
{
    public enum DataRarity
    {
        COMMON,
        RARE,
        EPIC,
    }

    [Export]
    public string Id { get; set; } = string.Empty;

    [Export]
    public string DisplayName { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public DataRarity Rarity { get; set; } = DataRarity.COMMON;

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

    public DataRarity rarity
    {
        get => Rarity;
        set => Rarity = value;
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
