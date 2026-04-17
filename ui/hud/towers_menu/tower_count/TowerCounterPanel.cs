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
        get => _icon;
        set
        {
            _icon = value;
            UpdateTexture();
        }
    }

    public int count
    {
        get => _count;
        set => SetCount(value);
    }

    public int max_value
    {
        get => _max_value;
        set
        {
            _max_value = value;
            if (max_label != null)
            {
                max_label.Text = _max_value.ToString();
            }
        }
    }

    private Label current_label;
    private TextureRect texture_rect;
    private Label max_label;

    public override void _Ready()
    {
        current_label = GetNodeOrNull<Label>("RedCount/Value/CurrentLabel");
        texture_rect = GetNodeOrNull<TextureRect>("RedCount/TextureRect");
        max_label = GetNodeOrNull<Label>("RedCount/Value/MaxLabel");

        UpdateTexture();
        if (texture_rect != null)
        {
            texture_rect.Texture = icon;
        }
    }

    private void UpdateTexture()
    {
        if (texture_rect != null)
        {
            texture_rect.Texture = icon;
        }
    }

    private void SetCount(int value)
    {
        if (value < TIER_1_COUNT && max_value < TIER_1_COUNT)
        {
            max_value = TIER_1_COUNT;
        }
        else if (value >= TIER_1_COUNT && value < TIER_2_COUNT && max_value < TIER_2_COUNT)
        {
            max_value = TIER_2_COUNT;
        }
        else if (value >= TIER_2_COUNT && value < TIER_3_COUNT && max_value < TIER_3_COUNT)
        {
            max_value = TIER_3_COUNT;
        }
        else if (value >= TIER_3_COUNT && value < TIER_4_COUNT && max_value < TIER_4_COUNT)
        {
            max_value = TIER_4_COUNT;
        }

        _count = value;
        if (_count <= TIER_4_COUNT && current_label != null)
        {
            current_label.Text = _count.ToString();
        }
    }
}
