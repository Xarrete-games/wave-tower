using Godot;

[Tool]
public partial class TowerStatUi : Control
{
    [Export]
    public string stat_name { get; set; } = string.Empty;

    [Export]
    public Texture2D stat_icon { get; set; }

    [Export]
    public float stat_value { get; set; }

    [Export]
    public bool is_float { get; set; } = false;

    [Export]
    public string unit { get; set; } = string.Empty;

    private TextureRect stat_texture;
    private Label value_label;
    private Label unit_label;
    private Label upgrade_value_label;

    public override void _Ready()
    {
        stat_texture = GetNodeOrNull<TextureRect>("MarginContainer/StatTexture");
        value_label = GetNodeOrNull<Label>("BoxContainer/ValueLabel");
        unit_label = GetNodeOrNull<Label>("BoxContainer/UnitLabel");
        upgrade_value_label = GetNodeOrNull<Label>("BoxContainer/UpgradeValueLabel");

        if (stat_texture != null)
        {
            stat_texture.Texture = stat_icon;
        }

        if (upgrade_value_label != null)
        {
            upgrade_value_label.Visible = false;
        }

        if (unit_label != null)
        {
            if (string.IsNullOrEmpty(unit))
            {
                unit_label.Visible = false;
            }
            else
            {
                unit_label.Text = unit;
            }
        }

        RefreshValueLabel();
    }

    public void set_value(float new_value)
    {
        stat_value = new_value;
        RefreshValueLabel();
    }

    public void show_upgrade_value(float upgrade_amount)
    {
        if (upgrade_value_label == null)
        {
            return;
        }

        upgrade_value_label.Visible = true;
        if (is_float)
        {
            upgrade_value_label.Text = "(+" + Mathf.Snapped(upgrade_amount, 0.01f) + ")";
        }
        else
        {
            upgrade_value_label.Text = "(+" + (int)upgrade_amount + ")";
        }
    }

    public void hide_upgrade_value()
    {
        if (upgrade_value_label != null)
        {
            upgrade_value_label.Visible = false;
        }
    }

    private void RefreshValueLabel()
    {
        if (value_label == null)
        {
            return;
        }

        if (is_float)
        {
            value_label.Text = Mathf.Snapped(stat_value, 0.01f).ToString();
        }
        else
        {
            value_label.Text = ((int)stat_value).ToString();
        }
    }
}
