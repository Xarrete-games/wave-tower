using Godot;

public partial class Game : Node2D
{
    private static readonly PackedScene BootScene = GD.Load<PackedScene>("uid://bfm0i7ehshgsf");

    [Export]
    public Godot.Collections.Array<string> LevelsPaths { get; set; } = new();

    [Export]
    public PackedScene Pause { get; set; }

    [Export]
    public bool TriggerFinishWave { get; set; } = false;

    private PauseMenu _pauseInstance;
    private MusicHandler _musicHandler;
    private CanvasLayer _configLayer;

    public override void _Ready()
    {
        _musicHandler = GetNode<MusicHandler>("MusicHandler");
        _configLayer = GetNode<CanvasLayer>("ConfigLayer");

        ClickEvents.ConfigButtonPressed += OpenConfigMenu;
        ClickEvents.ResetGameButtonPressed += ResetGame;

        RunContext runContext = RunContext.Instance;
        runContext.Progress.TotalLevels = LevelsPaths.Count;

        GameState gameState = GetNode<GameState>("/root/GameState");
        gameState.State = GameState.InGame;

        _musicHandler.PlayMusic();

        if (TriggerFinishWave)
        {
            runContext.Progress.NotifyCurrentWaveFinished();
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
        GetTree().ChangeSceneToPacked(BootScene);
    }

    public void OpenConfigMenu()
    {
        if (_pauseInstance != null && _pauseInstance.IsVisibleInTree())
        {
            return;
        }

        if (Pause == null)
        {
            GD.PushError("[Game] Pause scene is not assigned.");
            return;
        }

        if (_configLayer == null)
        {
            GD.PushError("[Game] ConfigLayer node is missing.");
            return;
        }

        GetTree().Paused = !GetTree().Paused;
        _pauseInstance = Pause.Instantiate<PauseMenu>();
        if (_pauseInstance == null)
        {
            GD.PushError("[Game] Pause scene did not instantiate a PauseMenu.");
            GetTree().Paused = false;
            return;
        }

        _pauseInstance.ResumeGame += CloseConfigMenu;
        _configLayer.AddChild(_pauseInstance);
    }

    public void CloseConfigMenu()
    {
        if (_pauseInstance == null || !_pauseInstance.IsVisibleInTree())
        {
            return;
        }

        _pauseInstance.ResumeGame -= CloseConfigMenu;
        _pauseInstance.QueueFree();
        _pauseInstance = null;
    }
}
