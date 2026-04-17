using Godot;

public partial class RelicUI : Control
{
    private static readonly Color SemiTransparentColor = new(1f, 1f, 1f, 0.5f);
    private static readonly Color OpaqueColor = new(1f, 1f, 1f, 1f);

    private TextureRect _texture;
    private Label _amountLabel;
    private AnimationPlayer _animationPlayer;

    public Relic relic;

    public override void _Ready()
    {
        _texture = GetNode<TextureRect>("VBoxContainer/MarginContainer/texture");
        _amountLabel = GetNode<Label>("VBoxContainer/MarginContainer/amount");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _animationPlayer.Play("on_enter");
    }

    public void SetRelic(Relic relicData)
    {
        RelicData data = relicData?.Data;
        if (relicData == null || data == null)
        {
            return;
        }

        relic = relicData;
        _texture.Texture = data.icon;

        _texture.Modulate = relicData.Disabled ? SemiTransparentColor : OpaqueColor;

        bool showCounter = data.show_counter;
        _amountLabel.Visible = showCounter;
        if (showCounter)
        {
            _amountLabel.Text = relicData.Counter.ToString();
        }
    }

    private void OnMouseEntered()
    {
        RelicData data = relic?.Data;
        if (data == null)
        {
            return;
        }

        string description = data.description;
        string displayName = data.DisplayName;
        HintManagerStatic.ShowHint(this, this, description, displayName, HintManagerStatic.PositionHint.BOTTOM);
    }

    private void OnMouseExited()
    {
        HintManagerStatic.RemoveHint(this);
    }

    private void OnMarginContainerMouseEntered()
    {
        OnMouseEntered();
    }

    private void OnMarginContainerMouseExited()
    {
        OnMouseExited();
    }
}