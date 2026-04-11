using Godot;

public partial class EnemyGenerator : Node
{
    private const int TOTAL_WAVES = 30;
    private static readonly Script WaveComposerScript = GD.Load<Script>("res://enemies/enemy_generator/wave_composer.gd");

    [Export]
    public Node wave_spawner;

    [Export]
    public Resource wave_config;

    [Export]
    public Node2D enemies_container;

    private int _waveNumber;
    private GodotObject _composer;
    private int _enemiesLeft;
    private bool _isTrackingEnemyExit;

    public override void _Ready()
    {
        var enemyCatalog = DataLoaderAccess.GetAllEnemies();

        if (this.wave_config == null)
        {
            this.wave_config = GD.Load<Resource>("res://enemies/enemy_generator/wave_config.tres");
        }

        this._composer = WaveComposerScript.Call("new", this.wave_config, enemyCatalog).AsGodotObject();
        if (this._composer == null)
        {
            GD.PushError("[EnemyGenerator] Could not instantiate WaveComposer.");
            return;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.total_waves = TOTAL_WAVES;

        if (this.wave_spawner != null)
        {
            this.wave_spawner.Set("enemies_container", this.enemies_container);
            this.wave_spawner.Connect("wave_started", Callable.From<int>(this.OnWaveStarted));
            this.wave_spawner.Connect("wave_finished", Callable.From<int>(this.OnWaveFinished));
            this.wave_spawner.Connect("enemy_spawned", Callable.From<Variant>(this.OnEnemySpawned));
        }

        ClickEventsBus.NextWavePressed += this.StartNextWave;
    }

    public override void _ExitTree()
    {
        ClickEventsBus.NextWavePressed -= this.StartNextWave;

        if (this._isTrackingEnemyExit && this.enemies_container != null)
        {
            this.enemies_container.ChildExitingTree -= this.OnEnemyLeft;
            this._isTrackingEnemyExit = false;
        }
    }

    public void StartNextWave()
    {
        if (this.wave_spawner == null)
        {
            GD.PushWarning("[EnemyGeneratorProcedural] No WaveSpawner assigned");
            return;
        }

        this._waveNumber += 1;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.current_wave = this._waveNumber;

        var groups = this._composer.Call("compose_wave", this._waveNumber).AsGodotArray<Variant>();

        int totalEnemies = 0;
        for (int index = 0; index < groups.Count; index++)
        {
            GodotObject group = groups[index].AsGodotObject();
            if (group == null)
            {
                continue;
            }

            var enemies = group.Get("enemies").AsGodotArray<Variant>();
            totalEnemies += enemies.Count;
        }

        int budget = (int)this._composer.Call("get_budget_for_wave", this._waveNumber);
        GD.Print($"[Wave {this._waveNumber}] Budget: {budget} | Groups: {groups.Count} | Total enemies: {totalEnemies}");

        this.wave_spawner.Call("start_wave", this._waveNumber, groups, this.wave_config);
    }

    private void OnWaveStarted(int waveNumber)
    {
        GD.Print($"[EnemyGeneratorProcedural] Wave {waveNumber} started");
    }

    private void OnWaveFinished(int waveNumber)
    {
        GD.Print($"[EnemyGeneratorProcedural] Wave {waveNumber} finished spawning");

        this._enemiesLeft = GetTree().GetNodesInGroup("enemy").Count;
        if (this._enemiesLeft == 0)
        {
            this.ReportFinished();
            return;
        }

        if (!this._isTrackingEnemyExit)
        {
            this.enemies_container.ChildExitingTree += this.OnEnemyLeft;
            this._isTrackingEnemyExit = true;
        }
    }

    private void OnEnemySpawned(Variant enemy)
    {
        GodotObject enemyObj = enemy.AsGodotObject();
        if (enemyObj == null)
        {
            return;
        }

        enemyObj.Connect("die", Callable.From<Variant, Variant>(this.OnEnemyDie));
        enemyObj.Connect("target_reached", Callable.From<Variant>(this.OnEnemyTargetReached));
    }

    private void OnEnemyLeft(Node node)
    {
        if (node.IsInGroup("enemy"))
        {
            this._enemiesLeft -= 1;
        }

        if (this._enemiesLeft <= 0)
        {
            if (this._isTrackingEnemyExit)
            {
                this.enemies_container.ChildExitingTree -= this.OnEnemyLeft;
                this._isTrackingEnemyExit = false;
            }

            this.CallDeferred(MethodName.ReportFinished);
        }
    }

    private void ReportFinished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        GameState gameState = GetNode<GameState>("/root/GameState");

        if (runContext.is_on_restarting || runContext.status.health <= 0 || gameState.is_on_main_menu())
        {
            return;
        }

        if (this._waveNumber >= TOTAL_WAVES)
        {
            runContext.progress.EmitSignal(RunProgress.SignalName.last_wave_finished);
            return;
        }

        this.SyncRuntimeStatusFromLegacy(runContext.status);
        Hooks.OnWaveFinished(Hooks.GetListenersFromRuntime());
        this.SyncLegacyStatusFromRuntime(runContext.status);

        runContext.progress.EmitSignal(RunProgress.SignalName.current_wave_finished);
    }

    private void SyncRuntimeStatusFromLegacy(Status status)
    {
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.max_health, status.health, status.armor);
    }

    private void SyncLegacyStatusFromRuntime(Status status)
    {
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.max_health != runtime.MaxHealth)
        {
            status.max_health = runtime.MaxHealth;
        }

        if (status.armor != runtime.Armor)
        {
            status.armor = runtime.Armor;
        }

        if (status.health != runtime.Health)
        {
            status.health = runtime.Health;
        }
    }

    private void OnEnemyTargetReached(Variant enemy)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        GodotObject enemyObj = enemy.AsGodotObject();
        if (enemyObj != null)
        {
            runContext.status.apply_damage((int)enemyObj.Get("damage"));
        }

        runContext.enemy_manager.EmitSignal(EnemyManager.SignalName.enemy_target_reached, enemy);
    }

    private void OnEnemyDie(Variant enemy, Variant attack)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.enemy_manager.EmitSignal(EnemyManager.SignalName.enemy_die, enemy, attack);
    }
}