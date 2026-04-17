using Godot;

public partial class RunContext : Node
{
    private static readonly PackedScene DEATH_SCENE = GD.Load<PackedScene>("uid://dcq16u6g6ahsp");

    [Export]
    public bool is_on_restarting = false;

    [Export]
    public Variant composite_tile_map;

    public OffersManager offers_manager;

    public RunProgress progress;

    public Economy economy;

    public Status status;

    public TowersManager towers_manager;

    public RelicsManager relics_manager;

    public ConsumablesManager consumables_manager;

    public EnemyManager enemy_manager;

    public override void _Ready()
    {
        reset_run();
    }

    public override void _ExitTree()
    {
        if (status != null)
        {
            status.player_died -= OnDie;
        }
    }

    public void reset_run()
    {
        RunContextRuntime.Reset();

        if (status != null)
        {
            status.player_died -= OnDie;
        }

        towers_manager?.dispose_events();

        offers_manager = new OffersManager();
        progress = new RunProgress();
        economy = new Economy();
        relics_manager = new RelicsManager();

        status = new Status();
        status.setup(progress, relics_manager);

        towers_manager = new TowersManager();
        towers_manager.setup(progress);

        consumables_manager = new ConsumablesManager();
        enemy_manager = new EnemyManager();
        is_on_restarting = false;

        status.player_died += OnDie;
    }

    private void OnDie()
    {
        Node deathScene = DEATH_SCENE.Instantiate();
        GetTree().Root.AddChild(deathScene);
    }
}
