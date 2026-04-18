using Godot;
using System.Collections.Generic;

public partial class LootScreen : Control
{
    [Export]
    public PackedScene LootScreenItemScene;

    [Export]
    public Control ItemsContainer;

    public override void _Ready()
    {
        ItemsContainer.ChildExitingTree += OnItemRemoved;
    }

    public void SetLoot(List<LootItemData> data)
    {
        foreach (LootItemData itemData in data)
        {
            LootScreenItem lootScreenItem = LootScreenItemScene.Instantiate<LootScreenItem>();
            ItemsContainer.AddChild(lootScreenItem);
            lootScreenItem.SetLootItem(itemData);
        }
    }

    private void OnXarreteActionButtonXarretaPressed()
    {
        QueueFree();
    }

    private void OnItemRemoved(Node _item)
    {
        if (ItemsContainer.GetChildCount() == 1)
        {
            QueueFree();
        }
    }
}