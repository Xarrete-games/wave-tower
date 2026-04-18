using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class LevelUpAlert : Control
{
    [Export] public float TimeToVanish = 3f;

    private Label _levelUp;

    public override async void _Ready()
    {
        _levelUp = GetNode<Label>("LevelUp");
        var tween1 = CreateTween();
        var randomNumber = (int)GD.RandRange(-100, 100);
        tween1.TweenProperty(_levelUp, "position", new Vector2(randomNumber, -30), TimeToVanish / 2f);

        var tween2 = CreateTween();
        tween2.TweenProperty(_levelUp, "modulate:a", 0.0f, TimeToVanish / 2f);
        await ToSignal(tween2, Tween.SignalName.Finished);
        QueueFree();
    }
}
