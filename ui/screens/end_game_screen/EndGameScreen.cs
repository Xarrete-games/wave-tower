using Godot;

[GlobalClass]
public partial class EndGameScreen : CanvasLayer
{
    private static readonly PackedScene MainMenuScene = GD.Load<PackedScene>("uid://4i6kl0xurgeg");

    private const float FadeDuration = 5.0f;

    private Panel _panel;
    private Label _label3;
    private bool _readyToExit;

    public override void _Ready()
    {
        GetNode<AudioManager>("/root/AudioManager").PlayMainPiano();

        _panel = GetNode<Panel>("Panel");
        _label3 = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer2/Label3");

        _label3.Visible = false;
        _panel.Modulate = Colors.White with { A = 1.0f };

        fade_in();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && _readyToExit)
        {
            GetTree().ChangeSceneToPacked(MainMenuScene);
        }
    }

    public void fade_in()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(_panel, "modulate:a", 0.0f, FadeDuration);
    }

    private void OnExitTimerTimeout()
    {
        _label3.Visible = true;
        _readyToExit = true;
    }
}
