using Godot;

[GlobalClass]
public partial class DebuffSlot : Control
{
    [Export] public TextureRect texture_rect;
    [Export] public Label label;

    private int _amount;
    public int amount
    {
        get => _amount;
        set
        {
            _amount = value;
            if (label != null)
            {
                label.Text = _amount.ToString();
            }
        }
    }

    private Texture2D _texture;
    public Texture2D texture
    {
        get => _texture;
        set
        {
            _texture = value;
            if (texture_rect != null)
            {
                texture_rect.Texture = value;
            }
        }
    }
}
