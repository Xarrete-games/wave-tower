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
        this.name_label = GetNodeOrNull<RichTextLabel>("MarginContainer/VBoxContainer/DescriptionContainer/NameLabel");
        this.description_label = GetNodeOrNull<Label>("MarginContainer/VBoxContainer/DescriptionContainer/DescriptionLabel");
        this.damage_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/DamageStatUi");
        this.attack_speed_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/AttkSpeedStatUi");
        this.range_stat = GetNodeOrNull<TowerStatUi>("MarginContainer/VBoxContainer/StatsContainer/RangeStatUi");
    }

    public void set_stats(TowerData configuration)
    {
        if (configuration == null)
        {
            return;
        }

        if (this.name_label != null)
        {
            this.name_label.Text = "[u]" + configuration.display_name + "[/u]";
        }

        if (this.description_label != null)
        {
            this.description_label.Text = configuration.description;
        }

        this.damage_stat?.set_value(configuration.base_damage);
        this.attack_speed_stat?.set_value(configuration.base_attack_speed);
        this.range_stat?.set_value(configuration.base_attack_range);
    }
}
