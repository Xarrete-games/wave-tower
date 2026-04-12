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

    public void set_stats(Variant configuration)
    {
        GodotObject cfg = configuration.AsGodotObject();
        if (cfg == null)
        {
            return;
        }

        if (this.name_label != null)
        {
            this.name_label.Text = "[u]" + cfg.Get("display_name").AsString() + "[/u]";
        }

        if (this.description_label != null)
        {
            this.description_label.Text = cfg.Get("description").AsString();
        }

        this.damage_stat?.set_value(cfg.Get("base_damage").AsSingle());
        this.attack_speed_stat?.set_value(cfg.Get("base_attack_speed").AsSingle());
        this.range_stat?.set_value(cfg.Get("base_attack_range").AsSingle());
    }
}
