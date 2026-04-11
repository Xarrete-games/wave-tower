using Godot;

public partial class InventoryUI : Control
{
    [Export]
    public Control slots_container;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.consumables_manager.Connect("consumable_added", Callable.From<Variant>(this.OnConsumableAdded));
    }

    private void OnConsumableAdded(Variant consumable)
    {
        foreach (Node slotNode in this.slots_container.GetChildren())
        {
            InventoryUISlot slot = slotNode as InventoryUISlot;
            if (slot != null && slot.IsEmpty())
            {
                slot.SetConsumable(consumable);
                return;
            }
        }
    }
}