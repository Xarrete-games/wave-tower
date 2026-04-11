using Godot;

public partial class LootScreen : Control
{
    [Export]
    public PackedScene loot_screen_item_scene;

    [Export]
    public Control items_container;

    public override void _Ready()
    {
        this.items_container.ChildExitingTree += this.OnItemRemoved;
    }

    public void SetLoot(Godot.Collections.Array<Variant> data)
    {
        foreach (Variant itemData in data)
        {
            LootScreenItem lootScreenItem = this.loot_screen_item_scene.Instantiate<LootScreenItem>();
            this.items_container.AddChild(lootScreenItem);
            lootScreenItem.SetLootItem(itemData);
        }
    }

    private void _on_xarrete_action_button_xarreta_pressed()
    {
        QueueFree();
    }

    private void OnItemRemoved(Node _item)
    {
        if (this.items_container.GetChildCount() == 1)
        {
            QueueFree();
        }
    }
}