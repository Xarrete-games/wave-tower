using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class GoldDropped : Control
{
    [Export] public float TimeToVanish = 2.0f;

    private Label _label;

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "GoldDropped.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        _label = GetNode<Label>("Label");

        Tween tween1 = CreateTween();
        tween1.TweenProperty(_label, "position", new Vector2(0, -10), TimeToVanish);

        Tween tween2 = CreateTween();
        tween2.TweenProperty(_label, "modulate:a", 0.0f, TimeToVanish);

        await ToSignal(tween2, Tween.SignalName.Finished);
        QueueFree();
    }

    public void SetGold(int newValue)
    {
        _label.Text = "+" + newValue;
    }
}
