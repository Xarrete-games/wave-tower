using Godot;

public partial class InventoryUISlot : Control
{
    private TextureRect _textureRect;
    private Consumable _consumable;
    private ConsumablesManager _consumablesManager;

    public override void _Ready()
    {
        this._textureRect = GetNode<TextureRect>("CenterContainer/TextureRect");
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._consumablesManager = runContext?.consumables_manager;
        if (this._consumablesManager != null)
        {
            this._consumablesManager.consumable_used += this.OnConsumableUsed;
        }
    }

    public override void _ExitTree()
    {
        if (this._consumablesManager != null)
        {
            this._consumablesManager.consumable_used -= this.OnConsumableUsed;
            this._consumablesManager = null;
        }
    }

    public bool IsEmpty()
    {
        return this._consumable == null;
    }

    public void SetConsumable(Consumable consumable)
    {
        this._consumable = consumable;
        ConsumableData data = consumable?.data;
        if (data != null)
        {
            this._textureRect.Texture = data.icon;
        }
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (this.IsEmpty())
        {
            return;
        }

        if (InputClickUtils.IsLeftClickReleased(@event))
        {
            GetNode<Node>("/root/AudioManager").Call("play_button_click");
            HintManagerStatic.RemoveHint(this);

            this._consumable?.emit_clicked();
        }
    }

    private void OnConsumableUsed(Consumable consumable)
    {
        if (this._consumable == null)
        {
            return;
        }

        if (ReferenceEquals(this._consumable, consumable))
        {
            this._consumable = null;
            this._textureRect.Texture = null;
        }
    }

    private void _on_mouse_entered()
    {
        if (this.IsEmpty())
        {
            return;
        }

        GetNode<Node>("/root/AudioManager").Call("play_button_hover");

        ConsumableData data = this._consumable?.data;
        if (data == null)
        {
            return;
        }

        string description = data.description;
        if (!string.IsNullOrEmpty(description))
        {
            string displayName = data.display_name;
            HintManagerStatic.ShowHint(this, this, description, displayName, HintManagerStatic.PositionHint.BOTTOM);
        }
    }

    private void _on_mouse_exited()
    {
        HintManagerStatic.RemoveHint(this);
    }
}