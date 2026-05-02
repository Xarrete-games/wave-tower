using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class DamageNumbers : Control
{
    [Export] public float TimeToVanish = 2.0f;

    private Label _damageLabel;

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "DamageNumbers.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        _damageLabel = GetNode<Label>("DamageLabel");

        Tween tween1 = CreateTween();
        int randomNumber = (int)GD.RandRange(-100, 100);
        tween1.TweenProperty(_damageLabel, "position", new Vector2(randomNumber, -30), TimeToVanish / 2.0f);

        Tween tween2 = CreateTween();
        tween2.TweenProperty(_damageLabel, "modulate:a", 0.0f, TimeToVanish / 2.0f);

        await ToSignal(tween2, Tween.SignalName.Finished);
        QueueFree();
    }

    public void SetAttack(Attack attack)
    {
        _damageLabel.Text = Mathf.RoundToInt(attack.Damage).ToString();
        if (attack.IsCritical || attack.IsExecution)
        {
            LabelSettings criticalSettings = _damageLabel.LabelSettings?.Duplicate() as LabelSettings;
            if (criticalSettings != null)
            {
                criticalSettings.FontColor = Colors.Red;
                _damageLabel.LabelSettings = criticalSettings;

                Tween tween = CreateTween();
                tween.TweenProperty(_damageLabel.LabelSettings, "font_size", 48, TimeToVanish);
            }
        }
    }
}
