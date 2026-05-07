using Godot;

public partial class LootScreenItem : Control
{
    [Export]
    public Texture2D GoldIcon;

    [Export]
    public TextureRect TextureRect;

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
            TextureRect.Texture = consumable.Icon;
            label.Text = consumable.DisplayName;
            return;
        }

        TextureRect.Texture = GoldIcon;
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
        RunContext runContext = RunContext.Instance;
        if (consumable != null)
        {
            bool isFull = runContext.ConsumablesManager.IsFull();
            if (isFull)
            {
                return;
            }

            Consumable consumableItem = consumable.CreateConsumable();
            runContext.ConsumablesManager.AddConsumable(consumableItem);
        }
        else
        {
            runContext.Economy.AddGold(_lootItemData.GoldAmount);
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
        string description = consumable == null ? string.Empty : consumable.Description;
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

        string description = consumable.Description;
        if (!string.IsNullOrEmpty(description))
        {
            HintManagerStatic.ShowHint(this, this, description, string.Empty, HintManagerStatic.PositionHint.RIGHT);
        }
    }
}