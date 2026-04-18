using Godot;

[GlobalClass]
public partial class GoldDropped : Control
{
    [Export] public float TimeToVanish = 2.0f;

    private Label label;

    public override async void _Ready()
    {
        label = GetNode<Label>("Label");

        Tween tween1 = CreateTween();
        tween1.TweenProperty(label, "position", new Vector2(0, -10), TimeToVanish);

        Tween tween2 = CreateTween();
        tween2.TweenProperty(label, "modulate:a", 0.0f, TimeToVanish);

        await ToSignal(tween2, Tween.SignalName.Finished);
        QueueFree();
    }

    public void set_gold(int new_value)
    {
        label.Text = "+" + new_value;
    }
}
