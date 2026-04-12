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
        this.stat_texture = GetNodeOrNull<TextureRect>("MarginContainer/StatTexture");
        this.value_label = GetNodeOrNull<Label>("BoxContainer/ValueLabel");
        this.unit_label = GetNodeOrNull<Label>("BoxContainer/UnitLabel");
        this.upgrade_value_label = GetNodeOrNull<Label>("BoxContainer/UpgradeValueLabel");

        if (this.stat_texture != null)
        {
            this.stat_texture.Texture = this.stat_icon;
        }

        if (this.upgrade_value_label != null)
        {
            this.upgrade_value_label.Visible = false;
        }

        if (this.unit_label != null)
        {
            if (string.IsNullOrEmpty(this.unit))
            {
                this.unit_label.Visible = false;
            }
            else
            {
                this.unit_label.Text = this.unit;
            }
        }

        this._refresh_value_label();
    }

    public void set_value(float new_value)
    {
        this.stat_value = new_value;
        this._refresh_value_label();
    }

    public void show_upgrade_value(float upgrade_amount)
    {
        if (this.upgrade_value_label == null)
        {
            return;
        }

        this.upgrade_value_label.Visible = true;
        if (this.is_float)
        {
            this.upgrade_value_label.Text = "(+" + Mathf.Snapped(upgrade_amount, 0.01f) + ")";
        }
        else
        {
            this.upgrade_value_label.Text = "(+" + (int)upgrade_amount + ")";
        }
    }

    public void hide_upgrade_value()
    {
        if (this.upgrade_value_label != null)
        {
            this.upgrade_value_label.Visible = false;
        }
    }

    private void _refresh_value_label()
    {
        if (this.value_label == null)
        {
            return;
        }

        if (this.is_float)
        {
            this.value_label.Text = Mathf.Snapped(this.stat_value, 0.01f).ToString();
        }
        else
        {
            this.value_label.Text = ((int)this.stat_value).ToString();
        }
    }
}
