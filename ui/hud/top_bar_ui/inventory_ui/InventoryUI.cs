using Godot;

public partial class InventoryUI : Control
{
    [Export]
    public Control slots_container;

    private ConsumablesManager _consumablesManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._consumablesManager = runContext?.consumables_manager;
        if (this._consumablesManager != null)
        {
            this._consumablesManager.consumable_added += this.OnConsumableAdded;
        }
    }

    public override void _ExitTree()
    {
        if (this._consumablesManager != null)
        {
            this._consumablesManager.consumable_added -= this.OnConsumableAdded;
            this._consumablesManager = null;
        }
    }

    private void OnConsumableAdded(Consumable consumable)
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