using Godot;

public partial class InventoryUI : Control
{
    [Export]
    public Control slots_container;

    private ConsumablesManager _consumablesManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _consumablesManager = runContext?.consumables_manager;
        if (_consumablesManager != null)
        {
            _consumablesManager.consumable_added += OnConsumableAdded;
        }
    }

    public override void _ExitTree()
    {
        if (_consumablesManager != null)
        {
            _consumablesManager.consumable_added -= OnConsumableAdded;
            _consumablesManager = null;
        }
    }

    private void OnConsumableAdded(Consumable consumable)
    {
        foreach (Node slotNode in slots_container.GetChildren())
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