using Godot;

public partial class Game : Node2D
{
    private static readonly PackedScene BootScene = GD.Load<PackedScene>("uid://bfm0i7ehshgsf");

    [Export]
    public Godot.Collections.Array<string> levels_paths { get; set; } = new();

    [Export]
    public PackedScene pause { get; set; }

    [Export]
    public bool trigger_finish_wave { get; set; } = false;

    private PauseMenu _pauseInstance;
    private MusicHandler _musicHandler;
    private CanvasLayer _configLayer;

    public override void _Ready()
    {
        _musicHandler = GetNode<MusicHandler>("MusicHandler");
        _configLayer = GetNode<CanvasLayer>("ConfigLayer");

        ClickEvents.ConfigButtonPressed += OpenConfigMenu;
        ClickEvents.ResetGameButtonPressed += ResetGame;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.total_levels = levels_paths.Count;

        GameState gameState = GetNode<GameState>("/root/GameState");
        gameState.state = GameState.IN_GAME;

        _musicHandler.play_music();

        if (trigger_finish_wave)
        {
            runContext.progress.notify_current_wave_finished();
        }
    }

    public override void _ExitTree()
    {
        ClickEvents.ConfigButtonPressed -= OpenConfigMenu;
        ClickEvents.ResetGameButtonPressed -= ResetGame;
    }

    public override void _Process(double delta)
    {
        if (!Input.IsActionJustPressed("exit"))
        {
            return;
        }

        ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
        if (actionManager.IsActionActive())
        {
            actionManager.EndAction();
        }
        else
        {
            OpenConfigMenu();
        }
    }

    public void ResetGame()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.is_on_restarting = true;
        GetTree().ChangeSceneToPacked(BootScene);
    }

    public void OpenConfigMenu()
    {
        if (_pauseInstance != null && _pauseInstance.IsVisibleInTree())
        {
            return;
        }

        GetTree().Paused = !GetTree().Paused;
        _pauseInstance = pause.Instantiate<PauseMenu>();
        _pauseInstance.resume_game += CloseConfigMenu;
        _configLayer.AddChild(_pauseInstance);
    }

    public void CloseConfigMenu()
    {
        if (_pauseInstance == null || !_pauseInstance.IsVisibleInTree())
        {
            return;
        }

        _pauseInstance.resume_game -= CloseConfigMenu;
        _pauseInstance.QueueFree();
        _pauseInstance = null;
    }
}
