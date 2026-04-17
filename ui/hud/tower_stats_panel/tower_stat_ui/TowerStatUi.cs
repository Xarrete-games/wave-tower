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

    private TextureRect _statTexture;
    private Label _valueLabel;
    private Label _unitLabel;
    private Label _upgradeValueLabel;

    public override void _Ready()
    {
        _statTexture = GetNodeOrNull<TextureRect>("MarginContainer/StatTexture");
        _valueLabel = GetNodeOrNull<Label>("BoxContainer/ValueLabel");
        _unitLabel = GetNodeOrNull<Label>("BoxContainer/UnitLabel");
        _upgradeValueLabel = GetNodeOrNull<Label>("BoxContainer/UpgradeValueLabel");

        if (_statTexture != null)
        {
            _statTexture.Texture = stat_icon;
        }

        if (_upgradeValueLabel != null)
        {
            _upgradeValueLabel.Visible = false;
        }

        if (_unitLabel != null)
        {
            if (string.IsNullOrEmpty(unit))
            {
                _unitLabel.Visible = false;
            }
            else
            {
                _unitLabel.Text = unit;
            }
        }

        RefreshValueLabel();
    }

    public void SetValue(float newValue)
    {
        stat_value = newValue;
        RefreshValueLabel();
    }

    public void ShowUpgradeValue(float upgradeAmount)
    {
        if (_upgradeValueLabel == null)
        {
            return;
        }

        _upgradeValueLabel.Visible = true;
        if (is_float)
        {
            _upgradeValueLabel.Text = "(+" + Mathf.Snapped(upgradeAmount, 0.01f) + ")";
        }
        else
        {
            _upgradeValueLabel.Text = "(+" + (int)upgradeAmount + ")";
        }
    }

    public void HideUpgradeValue()
    {
        if (_upgradeValueLabel != null)
        {
            _upgradeValueLabel.Visible = false;
        }
    }

    private void RefreshValueLabel()
    {
        if (_valueLabel == null)
        {
            return;
        }

        if (is_float)
        {
            _valueLabel.Text = Mathf.Snapped(stat_value, 0.01f).ToString();
        }
        else
        {
            _valueLabel.Text = ((int)stat_value).ToString();
        }
    }
}
