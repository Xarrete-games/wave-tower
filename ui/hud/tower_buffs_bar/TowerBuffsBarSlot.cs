using Godot;

public partial class TowerBuffsBarSlot : Control
{
    [Export]
    public NodePath texture;

    [Export]
    public NodePath value_label;

    public GodotObject tower_buff;

    private int _value = 0;
    public int value
    {
        get => this._value;
        set
        {
            this._value = value;
            if (this._valueLabelNode == null)
            {
                return;
            }

            this._valueLabelNode.Text = this._value != 0 ? this._value.ToString() : string.Empty;
        }
    }

    private TextureRect _textureNode;
    private Label _valueLabelNode;

    public override void _Ready()
    {
        this.ResolveNodes();

        if (this.tower_buff != null)
        {
            this.ApplyBuffVisuals();
        }

        this.value = this._value;
    }

    public void set_buff(Variant p_tower_buff, int p_value = 0)
    {
        this.tower_buff = p_tower_buff.AsGodotObject();
        this.value = p_value;

        this.ApplyBuffVisuals();
    }

    private void ResolveNodes()
    {
        if (this._textureNode == null)
        {
            this._textureNode = !this.texture.IsEmpty ? GetNodeOrNull<TextureRect>(this.texture) : GetNodeOrNull<TextureRect>("Texture");
        }

        if (this._valueLabelNode == null)
        {
            this._valueLabelNode = !this.value_label.IsEmpty ? GetNodeOrNull<Label>(this.value_label) : GetNodeOrNull<Label>("Label");
        }
    }

    private void ApplyBuffVisuals()
    {
        this.ResolveNodes();

        if (this._textureNode == null || this.tower_buff == null)
        {
            return;
        }

        GodotObject data = this.tower_buff.Get("data").AsGodotObject();
        if (data == null)
        {
            return;
        }

        this._textureNode.Texture = data.Get("icon").As<Texture2D>();
    }
}
