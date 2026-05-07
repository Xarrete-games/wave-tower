using Godot;

public partial class InventoryUI : Control
{
    [Export]
    public Control SlotsContainer;

    private ConsumablesManager _consumablesManager;

    public override void _Ready()
    {
        RunContext runContext = RunContext.Instance;
        _consumablesManager = runContext.ConsumablesManager;
        _consumablesManager.ConsumableAdded += OnConsumableAdded;
    }

    public override void _ExitTree()
    {
        if (_consumablesManager != null)
        {
            _consumablesManager.ConsumableAdded -= OnConsumableAdded;
            _consumablesManager = null;
        }
    }

    private void OnConsumableAdded(Consumable consumable)
    {
        foreach (Node slotNode in SlotsContainer.GetChildren())
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