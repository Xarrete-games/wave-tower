using Godot;

[Tool]
public partial class TowerStatUi : Control
{
    [Export]
    public string StatName { get; set; } = string.Empty;

    [Export]
    public Texture2D StatIcon { get; set; }

    [Export]
    public float StatValue { get; set; }

    [Export]
    public bool IsFloat { get; set; } = false;

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
            _statTexture.Texture = StatIcon;
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
        StatValue = newValue;
        RefreshValueLabel();
    }

    public void ShowUpgradeValue(float upgradeAmount)
    {
        if (_upgradeValueLabel == null)
        {
            return;
        }

        _upgradeValueLabel.Visible = true;
        if (IsFloat)
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

        if (IsFloat)
        {
            _valueLabel.Text = Mathf.Snapped(StatValue, 0.01f).ToString();
        }
        else
        {
            _valueLabel.Text = ((int)StatValue).ToString();
        }
    }
}
