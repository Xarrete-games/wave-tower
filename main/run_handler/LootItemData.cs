using Godot;

[GlobalClass]
public partial class LootItemData : RefCounted
{
    public int GoldAmount { get; set; }

    public GodotObject Consumable { get; set; }
}