using Godot;

public partial class RunContext : Node
{
    private static readonly PackedScene DEATH_SCENE = GD.Load<PackedScene>("uid://dcq16u6g6ahsp");

    [Export]
    public bool IsOnRestarting = false;

    [Export]
    public Variant CompositeTileMap;

    public OffersManager OffersManager;

    public RunProgress Progress;

    public Economy Economy;

    public Status Status;

    public TowersManager TowersManager;

    public RelicsManager RelicsManager;

    public ConsumablesManager ConsumablesManager;

    public EnemyManager EnemyManager;

    public override void _Ready()
    {
        ResetRun();
    }

    public override void _ExitTree()
    {
        if (Status != null)
        {
            Status.PlayerDied -= OnDie;
        }
    }

    public void ResetRun()
    {
        RunContextRuntime.Reset();

        if (Status != null)
        {
            Status.PlayerDied -= OnDie;
        }

        TowersManager?.DisposeEvents();

        OffersManager = new OffersManager();
        Progress = new RunProgress();
        Economy = new Economy();
        RelicsManager = new RelicsManager();

        Status = new Status();
        Status.Setup(Progress, RelicsManager);

        TowersManager = new TowersManager();
        TowersManager.Setup(Progress);

        ConsumablesManager = new ConsumablesManager();
        EnemyManager = new EnemyManager();
        IsOnRestarting = false;

        Status.PlayerDied += OnDie;
    }

    private void OnDie()
    {
        Node deathScene = DEATH_SCENE.Instantiate();
        GetTree().Root.AddChild(deathScene);
    }
}
