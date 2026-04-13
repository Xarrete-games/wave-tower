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
        this._texture = GetNode<TextureRect>("VBoxContainer/MarginContainer/texture");
        this._amountLabel = GetNode<Label>("VBoxContainer/MarginContainer/amount");
        this._animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        this._animationPlayer.Play("on_enter");
    }

    public void SetRelic(Relic relicData)
    {
        RelicData data = relicData?.Data;
        if (relicData == null || data == null)
        {
            return;
        }

        this.relic = relicData;
        this._texture.Texture = data.icon;

        this._texture.Modulate = relicData.Disabled ? SemiTransparentColor : OpaqueColor;

        bool showCounter = data.show_counter;
        this._amountLabel.Visible = showCounter;
        if (showCounter)
        {
            this._amountLabel.Text = relicData.Counter.ToString();
        }
    }

    private void _on_mouse_entered()
    {
        RelicData data = this.relic?.Data;
        if (data == null)
        {
            return;
        }

        string description = data.description;
        string displayName = data.display_name;
        HintManagerStatic.ShowHint(this, this, description, displayName, HintManagerStatic.PositionHint.BOTTOM);
    }

    private void _on_mouse_exited()
    {
        HintManagerStatic.RemoveHint(this);
    }

    private void _on_margin_container_mouse_entered()
    {
        this._on_mouse_entered();
    }

    private void _on_margin_container_mouse_exited()
    {
        this._on_mouse_exited();
    }
}