using Godot;

public sealed class RunContext
{
    private static readonly PackedScene DEATH_SCENE = GD.Load<PackedScene>("uid://dcq16u6g6ahsp");
    public static RunContext Instance { get; } = new RunContext();

    public CompositeTileMap CompositeTileMap { get; private set; }

    public OffersManager OffersManager { get; private set; }

    public RunProgress Progress { get; private set; }

    public Economy Economy { get; private set; }

    public Status Status { get; private set; }

    public TowersManager TowersManager { get; private set; }

    public RelicsManager RelicsManager { get; private set; }

    public ConsumablesManager ConsumablesManager { get; private set; }

    public EnemyManager EnemyManager { get; private set; }

    private RunContext()
    {
        ResetRun();
    }

    public void SetCompositeTileMap(CompositeTileMap compositeTileMap)
    {
        CompositeTileMap = compositeTileMap;
    }

    public void ResetRun()
    {
        RunContextRuntime.Reset();
        CompositeTileMap = null;

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

        Status.PlayerDied += OnDie;
    }

    private void OnDie()
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree?.Root == null)
        {
            return;
        }

        Node deathScene = DEATH_SCENE.Instantiate();
        tree.Root.AddChild(deathScene);
    }
}
