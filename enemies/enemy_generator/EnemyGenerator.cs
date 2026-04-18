using Godot;
using System.Collections.Generic;

public partial class EnemyGenerator : Node
{
    private const int TOTAL_WAVES = 30;

    [Export]
    public WaveSpawner WaveSpawner;

    [Export]
    public WaveConfig WaveConfig;

    [Export]
    public Node2D EnemiesContainer;

    private int _waveNumber;
    private WaveComposer _composer;
    private int _enemiesLeft;
    private bool _isTrackingEnemyExit;

    public override void _Ready()
    {
        var enemyCatalog = new Godot.Collections.Array<EnemyData>();
        List<EnemyData> rawEnemyCatalog = DataLoaderAccess.GetAllEnemies();
        for (int index = 0; index < rawEnemyCatalog.Count; index++)
        {
            EnemyData enemyData = rawEnemyCatalog[index];
            if (enemyData != null)
            {
                enemyCatalog.Add(enemyData);
            }
        }

        if (WaveConfig == null)
        {
            WaveConfig = GD.Load<WaveConfig>("res://enemies/enemy_generator/WaveConfig.tres");
        }

        _composer = new WaveComposer(WaveConfig, enemyCatalog);
        if (_composer == null)
        {
            GD.PushError("[EnemyGenerator] Could not instantiate WaveComposer.");
            return;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.total_waves = TOTAL_WAVES;

        if (WaveSpawner != null)
        {
            WaveSpawner.EnemiesContainer = EnemiesContainer;
            WaveSpawner.wave_started += OnWaveStarted;
            WaveSpawner.wave_finished += OnWaveFinished;
            WaveSpawner.enemy_spawned += OnEnemySpawned;
        }

        ClickEvents.NextWavePressed += StartNextWave;
    }

    public override void _ExitTree()
    {
        ClickEvents.NextWavePressed -= StartNextWave;

        if (_isTrackingEnemyExit && EnemiesContainer != null)
        {
            EnemiesContainer.ChildExitingTree -= OnEnemyLeft;
            _isTrackingEnemyExit = false;
        }
    }

    public void StartNextWave()
    {
        if (WaveSpawner == null)
        {
            GD.PushWarning("[EnemyGeneratorProcedural] No WaveSpawner assigned");
            return;
        }

        _waveNumber += 1;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.current_wave = _waveNumber;

        List<WaveComposer.WaveGroup> groups = _composer.compose_wave(_waveNumber);

        int totalEnemies = 0;
        for (int index = 0; index < groups.Count; index++)
        {
            totalEnemies += groups[index].enemies.Count;
        }

        int budget = _composer.get_budget_for_wave(_waveNumber);
        GD.Print($"[Wave {_waveNumber}] Budget: {budget} | Groups: {groups.Count} | Total enemies: {totalEnemies}");

        WaveSpawner.start_wave(_waveNumber, groups, WaveConfig);
    }

    private void OnWaveStarted(int waveNumber)
    {
        GD.Print($"[EnemyGeneratorProcedural] Wave {waveNumber} started");
    }

    private void OnWaveFinished(int waveNumber)
    {
        GD.Print($"[EnemyGeneratorProcedural] Wave {waveNumber} finished spawning");

        _enemiesLeft = GetTree().GetNodesInGroup("enemy").Count;
        if (_enemiesLeft == 0)
        {
            ReportFinished();
            return;
        }

        if (!_isTrackingEnemyExit)
        {
            EnemiesContainer.ChildExitingTree += OnEnemyLeft;
            _isTrackingEnemyExit = true;
        }
    }

    private void OnEnemySpawned(Enemy enemyObj)
    {
        if (enemyObj == null)
        {
            return;
        }

        enemyObj.Died += OnEnemyDie;
        enemyObj.TargetReached += OnEnemyTargetReached;
    }

    private void OnEnemyLeft(Node node)
    {
        if (node.IsInGroup("enemy"))
        {
            _enemiesLeft -= 1;
        }

        if (_enemiesLeft <= 0)
        {
            if (_isTrackingEnemyExit)
            {
                EnemiesContainer.ChildExitingTree -= OnEnemyLeft;
                _isTrackingEnemyExit = false;
            }

            CallDeferred(MethodName.ReportFinished);
        }
    }

    private void ReportFinished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        GameState gameState = GetNode<GameState>("/root/GameState");

        if (runContext.IsOnRestarting || runContext.status.health <= 0 || gameState.is_on_main_menu())
        {
            return;
        }

        if (_waveNumber >= TOTAL_WAVES)
        {
            runContext.progress.notify_last_wave_finished();
            return;
        }

        SyncRuntimeStatusFromLegacy(runContext.status);
        Hooks.OnWaveFinished(Hooks.GetListenersFromRuntime());
        SyncLegacyStatusFromRuntime(runContext.status);

        runContext.progress.notify_current_wave_finished();
    }

    private void SyncRuntimeStatusFromLegacy(Status status)
    {
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.MaxHealth, status.health, status.armor);
    }

    private void SyncLegacyStatusFromRuntime(Status status)
    {
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.MaxHealth != runtime.MaxHealth)
        {
            status.MaxHealth = runtime.MaxHealth;
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

    private void OnEnemyTargetReached(Enemy enemy)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (enemy != null)
        {
            runContext.status.apply_damage(enemy.damage);
        }

        runContext.enemy_manager.notify_enemy_target_reached(enemy);
    }

    private void OnEnemyDie(Enemy enemy, Attack attack)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.enemy_manager.notify_enemy_die(enemy, attack);
    }
}

