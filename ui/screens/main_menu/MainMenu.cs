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

        if (this.direct_init)
        {
            this._on_new_run_button_xarreta_pressed();
        }
    }

    private void _on_new_run_button_xarreta_pressed()
    {
        GetNode<AudioManager>("/root/AudioManager").stop_main_piano();
        this.CallDeferred(MethodName._init_game);
    }

    private void _on_credits_button_xarreta_pressed()
    {
        GetTree().Root.AddChild(CreditsScene.Instantiate());
    }

    private void _on_exit_button_xarreta_pressed()
    {
        GetTree().Quit();
    }

    private void _init_game()
    {
        GetTree().ChangeSceneToPacked(BootScene);
    }
}
