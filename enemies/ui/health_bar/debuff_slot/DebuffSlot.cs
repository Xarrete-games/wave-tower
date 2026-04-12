using Godot;

[GlobalClass]
public partial class DebuffSlot : Control
{
    [Export] public TextureRect texture_rect;
    [Export] public Label label;

    private int _amount;
    public int amount
    {
        get => this._amount;
        set
        {
            this._amount = value;
            if (this.label != null)
            {
                this.label.Text = this._amount.ToString();
            }
        }
    }

    private Texture2D _texture;
    public Texture2D texture
    {
        get => this._texture;
        set
        {
            this._texture = value;
            if (this.texture_rect != null)
            {
                this.texture_rect.Texture = value;
            }
        }
    }
}
