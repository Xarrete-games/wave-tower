using Godot;
using System.Collections.Generic;

public partial class LootScreen : Control
{
    [Export]
    public PackedScene loot_screen_item_scene;

    [Export]
    public Control items_container;

    public override void _Ready()
    {
        items_container.ChildExitingTree += OnItemRemoved;
    }

    public void SetLoot(List<LootItemData> data)
    {
        foreach (LootItemData itemData in data)
        {
            LootScreenItem lootScreenItem = loot_screen_item_scene.Instantiate<LootScreenItem>();
            items_container.AddChild(lootScreenItem);
            lootScreenItem.SetLootItem(itemData);
        }
    }

    private void OnXarreteActionButtonXarretaPressed()
    {
        QueueFree();
    }

    private void OnItemRemoved(Node _item)
    {
        if (items_container.GetChildCount() == 1)
        {
            QueueFree();
        }
    }
}