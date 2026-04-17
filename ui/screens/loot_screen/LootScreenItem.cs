using Godot;

public partial class LootScreenItem : Control
{
    [Export]
    public Texture2D gold_icon;

    [Export]
    public TextureRect texture_rect;

    [Export]
    public Label label;

    private LootItemData _lootItemData;

    public void SetLootItem(LootItemData lootItemData)
    {
        _lootItemData = lootItemData;
        if (_lootItemData == null)
        {
            return;
        }

        ConsumableData consumable = _lootItemData.Consumable;
        if (consumable != null)
        {
            texture_rect.Texture = consumable.icon;
            label.Text = consumable.DisplayName;
            return;
        }

        texture_rect.Texture = gold_icon;
        label.Text = $"{_lootItemData.GoldAmount} Gold";
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        if (_lootItemData == null)
        {
            return;
        }

        ConsumableData consumable = _lootItemData.Consumable;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (consumable != null)
        {
            bool isFull = runContext.consumables_manager.is_full();
            if (isFull)
            {
                return;
            }

            Consumable consumableItem = consumable.CreateConsumable();
            runContext.consumables_manager.add_consumable(consumableItem);
        }
        else
        {
            runContext.economy.add_gold(_lootItemData.GoldAmount);
        }

        QueueFree();
    }

    private void OnMouseExited()
    {
        StyleBoxFlat styleBox = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
        if (styleBox != null)
        {
            styleBox.BgColor = UIUtilsStatic.AccentColor;
            AddThemeStyleboxOverride("panel", styleBox);
        }

        ConsumableData consumable = _lootItemData?.Consumable;
        string description = consumable == null ? string.Empty : consumable.description;
        if (!string.IsNullOrEmpty(description))
        {
            HintManagerStatic.RemoveHint(this);
        }
    }

    private void OnMouseEntered()
    {
        StyleBoxFlat styleBox = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
        if (styleBox != null)
        {
            styleBox.BgColor = UIUtilsStatic.SecondaryColor;
            AddThemeStyleboxOverride("panel", styleBox);
        }

        ConsumableData consumable = _lootItemData?.Consumable;
        if (consumable == null)
        {
            return;
        }

        string description = consumable.description;
        if (!string.IsNullOrEmpty(description))
        {
            HintManagerStatic.ShowHint(this, this, description, string.Empty, HintManagerStatic.PositionHint.RIGHT);
        }
    }
}