using Godot;

public partial class LootScreenItem : Control
{
    [Export]
    public Texture2D gold_icon;

    [Export]
    public TextureRect texture_rect;

    [Export]
    public Label label;

    private Variant _lootItemData = default;

    public void SetLootItem(Variant lootItemData)
    {
        this._lootItemData = lootItemData;
        GodotObject lootObj = lootItemData.AsGodotObject();
        if (lootObj == null)
        {
            return;
        }

        Variant consumableVariant = lootObj.Get("consumable");
        GodotObject consumable = consumableVariant.AsGodotObject();
        if (consumable != null)
        {
            this.texture_rect.Texture = consumable.Get("icon").As<Texture2D>();
            this.label.Text = (string)consumable.Get("display_name");
            return;
        }

        this.texture_rect.Texture = this.gold_icon;
        this.label.Text = $"{(int)lootObj.Get("gold_amount")} Gold";
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        GodotObject lootObj = this._lootItemData.AsGodotObject();
        if (lootObj == null)
        {
            return;
        }

        GodotObject consumable = lootObj.Get("consumable").AsGodotObject();
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
            runContext.economy.Call("add_gold", (int)lootObj.Get("gold_amount"));
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

        GodotObject consumable = this._lootItemData.AsGodotObject()?.Get("consumable").AsGodotObject();
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

        GodotObject consumable = this._lootItemData.AsGodotObject()?.Get("consumable").AsGodotObject();
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