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
        this._musicHandler = GetNode<MusicHandler>("MusicHandler");
        this._configLayer = GetNode<CanvasLayer>("ConfigLayer");

        ClickEvents.ConfigButtonPressed += this.OpenConfigMenu;
        ClickEvents.ResetGameButtonPressed += this.ResetGame;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.total_levels = this.levels_paths.Count;

        GameState gameState = GetNode<GameState>("/root/GameState");
        gameState.state = GameState.IN_GAME;

        this._musicHandler.play_music();

        if (this.trigger_finish_wave)
        {
            runContext.progress.notify_current_wave_finished();
        }
    }

    public override void _ExitTree()
    {
        ClickEvents.ConfigButtonPressed -= this.OpenConfigMenu;
        ClickEvents.ResetGameButtonPressed -= this.ResetGame;
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
            this.OpenConfigMenu();
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
        if (this._pauseInstance != null && this._pauseInstance.IsVisibleInTree())
        {
            return;
        }

        GetTree().Paused = !GetTree().Paused;
        this._pauseInstance = this.pause.Instantiate<PauseMenu>();
        this._pauseInstance.resume_game += this.CloseConfigMenu;
        this._configLayer.AddChild(this._pauseInstance);
    }

    public void CloseConfigMenu()
    {
        if (this._pauseInstance == null || !this._pauseInstance.IsVisibleInTree())
        {
            return;
        }

        this._pauseInstance.resume_game -= this.CloseConfigMenu;
        this._pauseInstance.QueueFree();
        this._pauseInstance = null;
    }
}
