using Godot;

public partial class MainMenu : Control
{
    private static readonly PackedScene BootScene = GD.Load<PackedScene>("uid://bfm0i7ehshgsf");
    private static readonly PackedScene CreditsScene = GD.Load<PackedScene>("uid://bayb10jsajj4a");

    [Export]
    public bool direct_init = true;

    public override void _Ready()
    {
        GetNode<AudioManager>("/root/AudioManager").play_main_piano();
        GetNode<GameState>("/root/GameState").state = GameState.ON_MAIN_MENU;

        if (direct_init)
        {
            OnNewRunButtonXarretaPressed();
        }
    }

    private void OnNewRunButtonXarretaPressed()
    {
        GetNode<AudioManager>("/root/AudioManager").stop_main_piano();
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
