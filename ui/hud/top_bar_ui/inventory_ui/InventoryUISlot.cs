using Godot;

public partial class InventoryUISlot : Control
{
    private TextureRect _textureRect;
    private Consumable _consumable;
    private ConsumablesManager _consumablesManager;

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("CenterContainer/TextureRect");
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _consumablesManager = runContext?.ConsumablesManager;
        if (_consumablesManager != null)
        {
            _consumablesManager.ConsumableUsed += OnConsumableUsed;
        }
    }

    public override void _ExitTree()
    {
        if (_consumablesManager != null)
        {
            _consumablesManager.ConsumableUsed -= OnConsumableUsed;
            _consumablesManager = null;
        }
    }

    public bool IsEmpty()
    {
        return _consumable == null;
    }

    public void SetConsumable(Consumable consumable)
    {
        _consumable = consumable;
        ConsumableData data = consumable?.Data;
        if (data != null)
        {
            _textureRect.Texture = data.Icon;
        }
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (IsEmpty())
        {
            return;
        }

        if (InputClickUtils.IsLeftClickReleased(@event))
        {
            GetNode<AudioManager>("/root/AudioManager").PlayButtonClick();
            HintManagerStatic.RemoveHint(this);

            _consumable?.EmitClicked();
        }
    }

    private void OnConsumableUsed(Consumable consumable)
    {
        if (_consumable == null)
        {
            return;
        }

        if (ReferenceEquals(_consumable, consumable))
        {
            _consumable = null;
            _textureRect.Texture = null;
        }
    }

    private void OnMouseEntered()
    {
        if (IsEmpty())
        {
            return;
        }

        GetNode<AudioManager>("/root/AudioManager").PlayButtonHover();

        ConsumableData data = _consumable?.Data;
        if (data == null)
        {
            return;
        }

        string description = data.Description;
        if (!string.IsNullOrEmpty(description))
        {
            string displayName = data.DisplayName;
            HintManagerStatic.ShowHint(this, this, description, displayName, HintManagerStatic.PositionHint.BOTTOM);
        }
    }

    private void OnMouseExited()
    {
        HintManagerStatic.RemoveHint(this);
    }
}