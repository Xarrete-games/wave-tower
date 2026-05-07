using Godot;

public partial class MainMenu : Control
{
    private static readonly PackedScene BootScene = GD.Load<PackedScene>("uid://bfm0i7ehshgsf");
    private static readonly PackedScene CreditsScene = GD.Load<PackedScene>("uid://bayb10jsajj4a");

    [Export]
    public bool DirectInit = true;

    public override void _Ready()
    {
        GetNode<AudioManager>("/root/AudioManager").PlayMainPiano();
        GetNode<GameState>("/root/GameState").State = GameState.OnMainMenu;

        if (DirectInit)
        {
            OnNewRunButtonXarretaPressed();
        }
    }

    private void OnNewRunButtonXarretaPressed()
    {
        GetNode<AudioManager>("/root/AudioManager").StopMainPiano();
        CallDeferred(MethodName.InitGame);
    }

    private void OnCreditsButtonXarretaPressed()
    {
        GetTree().Root.AddChild(CreditsScene.Instantiate());
    }

    private void OnExitButtonXarretaPressed()
    {
        GetTree().Quit();
    }

    private void InitGame()
    {
        GetTree().ChangeSceneToPacked(BootScene);
    }
}
