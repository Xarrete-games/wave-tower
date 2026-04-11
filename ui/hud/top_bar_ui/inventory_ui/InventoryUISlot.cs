using Godot;

public partial class InventoryUISlot : Control
{
    private TextureRect _textureRect;
    private Variant _consumable = default;

    public override void _Ready()
    {
        this._textureRect = GetNode<TextureRect>("CenterContainer/TextureRect");
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.consumables_manager.Connect("consumable_used", Callable.From<Variant>(this.OnConsumableUsed));
    }

    public bool IsEmpty()
    {
        return this._consumable.VariantType == Variant.Type.Nil;
    }

    public void SetConsumable(Variant consumable)
    {
        this._consumable = consumable;
        GodotObject consumableObj = consumable.AsGodotObject();
        GodotObject data = consumableObj?.Get("data").AsGodotObject();
        if (data != null)
        {
            this._textureRect.Texture = data.Get("icon").As<Texture2D>();
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

            GodotObject consumableObj = this._consumable.AsGodotObject();
            consumableObj?.Call("emit_signal", "clicked", this._consumable);
        }
    }

    private void OnConsumableUsed(Variant consumable)
    {
        if (this._consumable.VariantType == Variant.Type.Nil)
        {
            return;
        }

        if (this._consumable.AsGodotObject() == consumable.AsGodotObject())
        {
            this._consumable = default;
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

        GodotObject consumableObj = this._consumable.AsGodotObject();
        GodotObject data = consumableObj?.Get("data").AsGodotObject();
        if (data == null)
        {
            return;
        }

        string description = (string)data.Get("description");
        if (!string.IsNullOrEmpty(description))
        {
            string displayName = (string)data.Get("display_name");
            HintManagerStatic.ShowHint(this, this, description, displayName, HintManagerStatic.PositionHint.BOTTOM);
        }
    }

    private void _on_mouse_exited()
    {
        HintManagerStatic.RemoveHint(this);
    }
}