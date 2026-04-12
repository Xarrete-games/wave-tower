using Godot;

public partial class TowerCounterPanel : Control
{
    private const int TIER_1_COUNT = 2;
    private const int TIER_2_COUNT = 4;
    private const int TIER_3_COUNT = 6;
    private const int TIER_4_COUNT = 8;

    private int _count = 0;
    private int _max_value = TIER_1_COUNT;
    private Texture2D _icon;

    [Export]
    public Texture2D icon
    {
        get => this._icon;
        set
        {
            this._icon = value;
            this._update_texture();
        }
    }

    public int count
    {
        get => this._count;
        set => this._set_count(value);
    }

    public int max_value
    {
        get => this._max_value;
        set
        {
            this._max_value = value;
            if (this.max_label != null)
            {
                this.max_label.Text = this._max_value.ToString();
            }
        }
    }

    private Label current_label;
    private TextureRect texture_rect;
    private Label max_label;

    public override void _Ready()
    {
        this.current_label = GetNodeOrNull<Label>("RedCount/Value/CurrentLabel");
        this.texture_rect = GetNodeOrNull<TextureRect>("RedCount/TextureRect");
        this.max_label = GetNodeOrNull<Label>("RedCount/Value/MaxLabel");

        this._update_texture();
        if (this.texture_rect != null)
        {
            this.texture_rect.Texture = this.icon;
        }
    }

    private void _update_texture()
    {
        if (this.texture_rect != null)
        {
            this.texture_rect.Texture = this.icon;
        }
    }

    private void _set_count(int value)
    {
        if (value < TIER_1_COUNT && this.max_value < TIER_1_COUNT)
        {
            this.max_value = TIER_1_COUNT;
        }
        else if (value >= TIER_1_COUNT && value < TIER_2_COUNT && this.max_value < TIER_2_COUNT)
        {
            this.max_value = TIER_2_COUNT;
        }
        else if (value >= TIER_2_COUNT && value < TIER_3_COUNT && this.max_value < TIER_3_COUNT)
        {
            this.max_value = TIER_3_COUNT;
        }
        else if (value >= TIER_3_COUNT && value < TIER_4_COUNT && this.max_value < TIER_4_COUNT)
        {
            this.max_value = TIER_4_COUNT;
        }

        this._count = value;
        if (this._count <= TIER_4_COUNT && this.current_label != null)
        {
            this.current_label.Text = this._count.ToString();
        }
    }
}
