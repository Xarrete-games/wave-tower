using Godot;

public partial class RelicUI : Control
{
    private static readonly Color SemiTransparentColor = new(1f, 1f, 1f, 0.5f);
    private static readonly Color OpaqueColor = new(1f, 1f, 1f, 1f);

    private TextureRect _texture;
    private Label _amountLabel;
    private AnimationPlayer _animationPlayer;

    public Variant relic;

    public override void _Ready()
    {
        this._texture = GetNode<TextureRect>("VBoxContainer/MarginContainer/texture");
        this._amountLabel = GetNode<Label>("VBoxContainer/MarginContainer/amount");
        this._animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        this._animationPlayer.Play("on_enter");
    }

    public void SetRelic(Variant relicData)
    {
        GodotObject relicObject = relicData.AsGodotObject();
        GodotObject data = relicObject?.Get("data").AsGodotObject();
        if (relicObject == null || data == null)
        {
            return;
        }

        this.relic = relicData;
        this._texture.Texture = data.Get("icon").As<Texture2D>();

        bool disabled = (bool)relicObject.Get("disabled");
        this._texture.Modulate = disabled ? SemiTransparentColor : OpaqueColor;

        bool showCounter = (bool)data.Get("show_counter");
        this._amountLabel.Visible = showCounter;
        if (showCounter)
        {
            this._amountLabel.Text = ((int)relicObject.Get("counter")).ToString();
        }
    }

    private void _on_mouse_entered()
    {
        GodotObject relicObject = this.relic.AsGodotObject();
        GodotObject data = relicObject?.Get("data").AsGodotObject();
        if (data == null)
        {
            return;
        }

        string description = (string)data.Get("description");
        string displayName = (string)data.Get("display_name");
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