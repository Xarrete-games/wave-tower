using Godot;

[GlobalClass]
public partial class DamageNumbers : Control
{
    [Export] public float time_to_vanish = 2.0f;

    private Label damage_label;

    public override async void _Ready()
    {
        this.damage_label = GetNode<Label>("DamageLabel");

        Tween tween1 = CreateTween();
        int randomNumber = (int)GD.RandRange(-100, 100);
        tween1.TweenProperty(this.damage_label, "position", new Vector2(randomNumber, -30), this.time_to_vanish / 2.0f);

        Tween tween2 = CreateTween();
        tween2.TweenProperty(this.damage_label, "modulate:a", 0.0f, this.time_to_vanish / 2.0f);

        await ToSignal(tween2, Tween.SignalName.Finished);
        QueueFree();
    }

    public void set_attack(Attack attack)
    {
        this.damage_label.Text = Mathf.RoundToInt(attack.damage).ToString();
        if (attack.is_critical || attack.is_execution)
        {
            LabelSettings criticalSettings = this.damage_label.LabelSettings?.Duplicate() as LabelSettings;
            if (criticalSettings != null)
            {
                criticalSettings.FontColor = Colors.Red;
                this.damage_label.LabelSettings = criticalSettings;

                Tween tween = CreateTween();
                tween.TweenProperty(this.damage_label.LabelSettings, "font_size", 48, this.time_to_vanish);
            }
        }
    }
}
