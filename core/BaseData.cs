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

    public virtual Variant create_item()
    {
        return default;
    }
}
