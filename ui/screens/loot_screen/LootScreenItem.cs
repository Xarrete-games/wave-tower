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

        GodotObject consumable = this._lootItemData.Consumable;
        if (consumable != null)
        {
            this.texture_rect.Texture = consumable.Get("icon").As<Texture2D>();
            this.label.Text = (string)consumable.Get("display_name");
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

        GodotObject consumable = this._lootItemData.Consumable;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (consumable != null)
        {
            bool isFull = (bool)runContext.consumables_manager.Call("is_full");
            if (isFull)
            {
                return;
            }

            Variant consumableItem = consumable.Call("create_item");
            runContext.consumables_manager.Call("add_consumable", consumableItem);
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

        GodotObject consumable = this._lootItemData?.Consumable;
        GodotObject consumableData = consumable;
        string description = consumableData == null ? string.Empty : (string)consumableData.Get("description");
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

        GodotObject consumable = this._lootItemData?.Consumable;
        if (consumable == null)
        {
            return;
        }

        string description = (string)consumable.Get("description");
        if (!string.IsNullOrEmpty(description))
        {
            HintManagerStatic.ShowHint(this, this, description, string.Empty, HintManagerStatic.PositionHint.RIGHT);
        }
    }
}