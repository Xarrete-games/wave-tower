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
        GetNode<Node>("/root/AudioManager").Call("play_main_piano");

        this._panel = GetNode<Panel>("Panel");
        this._label3 = GetNode<Label>("PanelContainer/CenterContainer/VBoxContainer2/Label3");

        this._label3.Visible = false;
        this._panel.Modulate = Colors.White with { A = 1.0f };

        this.fade_in();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && this._readyToExit)
        {
            GetTree().ChangeSceneToPacked(MainMenuScene);
        }
    }

    public void fade_in()
    {
        Tween tween = CreateTween();
        tween.TweenProperty(this._panel, "modulate:a", 0.0f, FadeDuration);
    }

    private void _on_exit_timer_timeout()
    {
        this._label3.Visible = true;
        this._readyToExit = true;
    }
}
