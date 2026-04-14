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
        this._lootItemData = lootItemData;
        if (this._lootItemData == null)
        {
            return;
        }

        ConsumableData consumable = this._lootItemData.Consumable;
        if (consumable != null)
        {
            this.texture_rect.Texture = consumable.icon;
            this.label.Text = consumable.display_name;
            return;
        }

        this.texture_rect.Texture = this.gold_icon;
        this.label.Text = $"{this._lootItemData.GoldAmount} Gold";
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        if (this._lootItemData == null)
        {
            return;
        }

        ConsumableData consumable = this._lootItemData.Consumable;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (consumable != null)
        {
            bool isFull = runContext.consumables_manager.is_full();
            if (isFull)
            {
                return;
            }

            Consumable consumableItem = consumable.create_consumable();
            runContext.consumables_manager.add_consumable(consumableItem);
        }
        else
        {
            runContext.economy.add_gold(this._lootItemData.GoldAmount);
        }

        QueueFree();
    }

    private void _on_mouse_exited()
    {
        StyleBoxFlat styleBox = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
        if (styleBox != null)
        {
            styleBox.BgColor = UIUtilsStatic.AccentColor;
            AddThemeStyleboxOverride("panel", styleBox);
        }

        ConsumableData consumable = this._lootItemData?.Consumable;
        string description = consumable == null ? string.Empty : consumable.description;
        if (!string.IsNullOrEmpty(description))
        {
            HintManagerStatic.RemoveHint(this);
        }
    }

    private void _on_mouse_entered()
    {
        StyleBoxFlat styleBox = GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
        if (styleBox != null)
        {
            styleBox.BgColor = UIUtilsStatic.SecondaryColor;
            AddThemeStyleboxOverride("panel", styleBox);
        }

        ConsumableData consumable = this._lootItemData?.Consumable;
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