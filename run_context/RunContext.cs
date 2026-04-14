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

    [Export]
    public TowersManager towers_manager;

    public RelicsManager relics_manager;

    [Export]
    public ConsumablesManager consumables_manager;

    public EnemyManager enemy_manager;

    public override void _Ready()
    {
        this.reset_run();
    }

    public override void _ExitTree()
    {
        if (this.status != null)
        {
            this.status.player_died -= this._on_die;
        }
    }

    public void reset_run()
    {
        RunContextRuntime.Reset();

        if (this.status != null)
        {
            this.status.player_died -= this._on_die;
        }

        this.towers_manager?.dispose_events();

        this.offers_manager = new OffersManager();
        this.progress = new RunProgress();
        this.economy = new Economy();
        this.relics_manager = new RelicsManager();

        this.status = new Status();
        this.status.setup(this.progress, this.relics_manager);

        this.towers_manager = new TowersManager();
        this.towers_manager.setup(this.progress);

        this.consumables_manager = new ConsumablesManager();
        this.enemy_manager = new EnemyManager();
        this.is_on_restarting = false;

        this.status.player_died += this._on_die;
    }

    private void _on_die()
    {
        Node deathScene = DEATH_SCENE.Instantiate();
        this.GetTree().Root.AddChild(deathScene);
    }
}
