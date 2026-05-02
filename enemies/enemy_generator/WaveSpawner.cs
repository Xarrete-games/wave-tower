using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class WaveSpawner : Node
{
    public event Action<int> WaveStarted;
    public event Action<Enemy> EnemySpawned;
    public event Action<int, int> GroupFinished;
    public event Action<int> WaveFinished;

    [Export]
    public WorldMap WorldMap;

    [Export]
    public PackedScene FallbackEnemyScene;

    [Export]
    public float PathOffsetYMin = -50.0f;

    [Export]
    public float PathOffsetYMax = 50.0f;

    [Export]
    public Node2D EnemiesContainer;

    private bool _isSpawning;

    public void StartWave(int waveNumber, List<WaveComposer.WaveGroup> groups, WaveConfig config)
    {
        AsyncTaskHelper.FireAndForget(StartWaveAsync(waveNumber, groups, config), "WaveSpawner.StartWaveAsync");
    }

    private async Task StartWaveAsync(int waveNumber, List<WaveComposer.WaveGroup> groups, WaveConfig config)
    {
        if (_isSpawning)
        {
            GD.PushWarning("[WaveSpawner] Already spawning a wave - ignoring request");
            return;
        }

        if (WorldMap == null)
        {
            GD.PushWarning("[WaveSpawner] No WorldMap assigned");
            return;
        }

        if (config == null)
        {
            GD.PushWarning("[WaveSpawner] No WaveConfig provided");
            return;
        }

        _isSpawning = true;
        WaveStarted?.Invoke(waveNumber);

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            WaveComposer.WaveGroup group = groups[groupIndex];
            if (group == null)
            {
                continue;
            }

            List<EnemyData> enemies = group.Enemies;
            int pressure = (int)group.Pressure;

            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
                SpawnSingle(enemies[enemyIndex]);

                if (enemyIndex < enemies.Count - 1)
                {
                    float spawnInterval = PickSpawnInterval(pressure, waveNumber, config);
                    await ToSignal(GetTree().CreateTimer(spawnInterval, false), SceneTreeTimer.SignalName.Timeout);
                }
            }

            GroupFinished?.Invoke(groupIndex, pressure);

            if (groupIndex < groups.Count - 1)
            {
                float groupDelay = config.GroupDelay;
                await ToSignal(GetTree().CreateTimer(groupDelay, false), SceneTreeTimer.SignalName.Timeout);
            }
        }

        _isSpawning = false;
        WaveFinished?.Invoke(waveNumber);
    }

    private float PickSpawnInterval(int pressure, int waveNumber, WaveConfig config)
    {
        Vector2 intervalRange = GetSpawnIntervalRange(pressure, config);
        Vector2 decayedRange = GetDecayedSpawnIntervalRange(intervalRange, waveNumber, config);
        float minSpawnInterval = GetMinSpawnInterval(config);
        float minInterval = Mathf.Max(decayedRange.X, minSpawnInterval);
        float maxInterval = Mathf.Max(decayedRange.Y, minInterval);
        maxInterval = Mathf.Max(maxInterval, minInterval);

        if (Mathf.IsEqualApprox(minInterval, maxInterval))
        {
            return minInterval;
        }

        return (float)GD.RandRange(minInterval, maxInterval);
    }

    private Vector2 GetSpawnIntervalRange(int pressure, WaveConfig config)
    {
        return pressure switch
        {
            0 => new Vector2(config.SpawnIntervalSwarmMin, config.SpawnIntervalSwarmMax),
            1 => new Vector2(config.SpawnIntervalSpeedMin, config.SpawnIntervalSpeedMax),
            2 => new Vector2(config.SpawnIntervalTankMin, config.SpawnIntervalTankMax),
            _ => new Vector2(config.SpawnIntervalNormalMin, config.SpawnIntervalNormalMax),
        };
    }

    private Vector2 GetDecayedSpawnIntervalRange(Vector2 baseRange, int waveNumber, WaveConfig config)
    {
        int everyWaves = Mathf.Max(config.SpawnIntervalMaxDecayEveryWaves, 1);
        int decaySteps = Mathf.Max((waveNumber - 1) / everyWaves, 0);
        float decayAmount = decaySteps * config.SpawnIntervalMaxDecayAmount;
        float minSpawnInterval = GetMinSpawnInterval(config);

        float decayedMin = Mathf.Max(baseRange.X - decayAmount, minSpawnInterval);
        float decayedMax = Mathf.Max(baseRange.Y - decayAmount, minSpawnInterval);
        if (decayedMax < decayedMin)
        {
            decayedMax = decayedMin;
        }

        return new Vector2(decayedMin, decayedMax);
    }

    private float GetMinSpawnInterval(WaveConfig config)
    {
        return Mathf.Max(config.SpawnIntervalMinCap, 0.01f);
    }

    private void SpawnSingle(EnemyData data)
    {
        if (data == null)
        {
            return;
        }

        IReadOnlyList<SpawnEntry> portalEntries = WorldMap.GetPortalEntries();
        if (portalEntries.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No spawn points available");
            return;
        }

        int portalIndex = (int)(GD.Randi() % (uint)portalEntries.Count);
        SpawnEntry spawnEntry = portalEntries[portalIndex];
        List<Vector2> waypoints = WorldMap.GetWaypointsForSpawnList(spawnEntry);
        if (waypoints.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No waypoints for spawn entry");
            return;
        }

        List<Vector2> enemyWaypoints = BuildEnemyWaypointsWithOffset(waypoints);
        PackedScene scene = data.Scene ?? FallbackEnemyScene;
        if (scene == null)
        {
            GD.PushError($"[WaveSpawner] No scene for enemy '{data.Name}' and no fallback set");
            return;
        }

        Enemy enemy = scene.Instantiate<Enemy>();
        if (enemy == null)
        {
            GD.PushError($"[WaveSpawner] Scene for '{data.Name}' did not produce an Enemy");
            return;
        }

        ApplyStats(enemy, data);
        enemy.AddToGroup("enemy");

        if (EnemiesContainer == null)
        {
            GD.PushWarning("[WaveSpawner] EnemiesContainer is null");
            enemy.QueueFree();
            return;
        }

        EnemiesContainer.AddChild(enemy);
        enemy.Disable();
        enemy.GlobalPosition = enemyWaypoints[0];
        enemy.Enable();
        enemy.SetWaypoints(enemyWaypoints);

        EnemySpawned?.Invoke(enemy);
    }

    private List<Vector2> BuildEnemyWaypointsWithOffset(IReadOnlyList<Vector2> baseWaypoints)
    {
        if (baseWaypoints.Count == 0)
        {
            return new List<Vector2>();
        }

        float minY = Mathf.Min(PathOffsetYMin, PathOffsetYMax);
        float maxY = Mathf.Max(PathOffsetYMin, PathOffsetYMax);
        float offsetY = (float)GD.RandRange(minY, maxY);
        Vector2 offset = new(0.0f, offsetY);

        var result = new List<Vector2>(baseWaypoints.Count);
        for (int index = 0; index < baseWaypoints.Count; index++)
        {
            result.Add(baseWaypoints[index] + offset);
        }

        return result;
    }

    private void ApplyStats(Enemy enemy, EnemyData data)
    {
        enemy.MaxHealth = data.MaxHealth;
        enemy.BaseSpeed = data.BaseSpeed;
        enemy.Damage = data.Damage;
        enemy.GoldValue = data.BaseGoldValue;
    }
}

