using Godot;

public partial class TowerButtonHint : Control
{
    private RichTextLabel name_label;
    private Label description_label;
    private TowerStatUi damage_stat;
    private TowerStatUi attack_speed_stat;
    private TowerStatUi range_stat;

    public override void _Ready()
    {
        name_label = GetNodeOrNull<RichTextLabel>("MarginContainer/VBoxContainer/DescriptionContainer/NameLabel");
        description_label = GetNodeOrNull<Label>("MarginContainer/VBoxContainer/DescriptionContainer/DescriptionLabel");
        damage_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/DamageStatUi");
        attack_speed_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/AttkSpeedStatUi");
        range_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/RangeStatUi");
    }

    public void set_stats(TowerData configuration)
    {
        if (configuration == null)
        {
            return;
        }

        if (name_label != null)
        {
            name_label.Text = "[u]" + configuration.display_name + "[/u]";
        }

        if (description_label != null)
        {
            description_label.Text = configuration.description;
        }

        damage_stat?.set_value(configuration.base_damage);
        attack_speed_stat?.set_value(configuration.base_attack_speed);
        range_stat?.set_value(configuration.base_attack_range);
    }
}
