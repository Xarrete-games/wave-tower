using Godot;

public partial class TowerButtonHint : Control
{
    private RichTextLabel _nameLabel;
    private Label _descriptionLabel;
    private TowerStatUi _damageStat;
    private TowerStatUi _attackSpeedStat;
    private TowerStatUi _rangeStat;

    public override void _Ready()
    {
        _nameLabel = GetNodeOrNull<RichTextLabel>("MarginContainer/VBoxContainer/DescriptionContainer/NameLabel");
        _descriptionLabel = GetNodeOrNull<Label>("MarginContainer/VBoxContainer/DescriptionContainer/DescriptionLabel");
        _damageStat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/DamageStatUi");
        _attackSpeedStat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/AttkSpeedStatUi");
        _rangeStat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/RangeStatUi");
    }

    public void SetStats(TowerData configuration)
    {
        if (configuration == null)
        {
            return;
        }

        if (_nameLabel != null)
        {
            _nameLabel.Text = "[u]" + configuration.DisplayName + "[/u]";
        }

        if (_descriptionLabel != null)
        {
            _descriptionLabel.Text = configuration.Description;
        }

        _damageStat?.SetValue(configuration.BaseDamage);
        _attackSpeedStat?.SetValue(configuration.BaseAttackSpeed);
        _rangeStat?.SetValue(configuration.BaseAttackRange);
    }
}
