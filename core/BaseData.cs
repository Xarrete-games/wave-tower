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

    public virtual object CreateItem()
    {
        return null;
    }
}
