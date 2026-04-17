using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class WaveSpawner : Node
{
    public event Action<int> wave_started;
    public event Action<Enemy> enemy_spawned;
    public event Action<int, int> group_finished;
    public event Action<int> wave_finished;

    [Export]
    public WorldMap world_map;

    [Export]
    public PackedScene fallback_enemy_scene;

    [Export]
    public float path_offset_y_min = -50.0f;

    [Export]
    public float path_offset_y_max = 50.0f;

    [Export]
    public Node2D enemies_container;

    private bool _is_spawning;

    public async void start_wave(int wave_number, List<WaveComposer.WaveGroup> groups, WaveConfig config)
    {
        if (_is_spawning)
        {
            GD.PushWarning("[WaveSpawner] Already spawning a wave - ignoring request");
            return;
        }

        if (world_map == null)
        {
            GD.PushWarning("[WaveSpawner] No world_map assigned");
            return;
        }

        if (config == null)
        {
            GD.PushWarning("[WaveSpawner] No WaveConfig provided");
            return;
        }

        _is_spawning = true;
        wave_started?.Invoke(wave_number);

        for (int groupIndex = 0; groupIndex < groups.Count; groupIndex++)
        {
            WaveComposer.WaveGroup group = groups[groupIndex];
            if (group == null)
            {
                continue;
            }

            Godot.Collections.Array<EnemyData> enemies = group.enemies;
            int pressure = (int)group.pressure;

            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
                SpawnSingle(enemies[enemyIndex]);

                if (enemyIndex < enemies.Count - 1)
                {
                    float spawnInterval = PickSpawnInterval(pressure, wave_number, config);
                    await ToSignal(GetTree().CreateTimer(spawnInterval, false), SceneTreeTimer.SignalName.Timeout);
                }
            }

            group_finished?.Invoke(groupIndex, pressure);

            if (groupIndex < groups.Count - 1)
            {
                float groupDelay = config.GroupDelay;
                await ToSignal(GetTree().CreateTimer(groupDelay, false), SceneTreeTimer.SignalName.Timeout);
            }
        }

        _is_spawning = false;
        wave_finished?.Invoke(wave_number);
    }

    private float PickSpawnInterval(int pressure, int wave_number, WaveConfig config)
    {
        Vector2 intervalRange = GetSpawnIntervalRange(pressure, config);
        Vector2 decayedRange = GetDecayedSpawnIntervalRange(intervalRange, wave_number, config);
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

    private Vector2 GetDecayedSpawnIntervalRange(Vector2 base_range, int wave_number, WaveConfig config)
    {
        int everyWaves = Mathf.Max(config.SpawnIntervalMaxDecayEveryWaves, 1);
        int decaySteps = Mathf.Max((wave_number - 1) / everyWaves, 0);
        float decayAmount = decaySteps * config.SpawnIntervalMaxDecayAmount;
        float minSpawnInterval = GetMinSpawnInterval(config);

        float decayedMin = Mathf.Max(base_range.X - decayAmount, minSpawnInterval);
        float decayedMax = Mathf.Max(base_range.Y - decayAmount, minSpawnInterval);
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

        Godot.Collections.Array<Godot.Collections.Dictionary> portalEntries = world_map.portal_entries;
        if (portalEntries.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No spawn points available");
            return;
        }

        int portalIndex = (int)(GD.Randi() % (uint)portalEntries.Count);
        Godot.Collections.Dictionary spawnEntry = portalEntries[portalIndex];
        Godot.Collections.Array<Vector2> waypoints = world_map.get_waypoints_for_spawn(spawnEntry);
        if (waypoints.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No waypoints for spawn entry");
            return;
        }

        Godot.Collections.Array<Vector2> enemyWaypoints = _build_enemy_waypoints_with_offset(waypoints);
        PackedScene scene = data.scene ?? fallback_enemy_scene;
        if (scene == null)
        {
            GD.PushError($"[WaveSpawner] No scene for enemy '{data.name}' and no fallback set");
            return;
        }

        Enemy enemy = scene.Instantiate<Enemy>();
        if (enemy == null)
        {
            GD.PushError($"[WaveSpawner] Scene for '{data.name}' did not produce an Enemy");
            return;
        }

        ApplyStats(enemy, data);
        enemy.AddToGroup("enemy");
        enemy.enabled = false;

        if (enemies_container == null)
        {
            GD.PushWarning("[WaveSpawner] enemies_container is null");
            enemy.QueueFree();
            return;
        }

        enemies_container.AddChild(enemy);
        enemy.disable();
        enemy.GlobalPosition = enemyWaypoints[0];
        enemy.enable();
        enemy.set_waypoints(enemyWaypoints);

        enemy_spawned?.Invoke(enemy);
    }

    private Godot.Collections.Array<Vector2> _build_enemy_waypoints_with_offset(Godot.Collections.Array<Vector2> base_waypoints)
    {
        if (base_waypoints.Count == 0)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        float minY = Mathf.Min(path_offset_y_min, path_offset_y_max);
        float maxY = Mathf.Max(path_offset_y_min, path_offset_y_max);
        float offsetY = (float)GD.RandRange(minY, maxY);
        Vector2 offset = new(0.0f, offsetY);

        var result = new Godot.Collections.Array<Vector2>();
        result.Resize(base_waypoints.Count);
        for (int index = 0; index < base_waypoints.Count; index++)
        {
            result[index] = base_waypoints[index] + offset;
        }

        return result;
    }

    private void ApplyStats(Enemy enemy, EnemyData data)
    {
        enemy.MaxHealth = data.MaxHealth;
        enemy.BaseSpeed = data.BaseSpeed;
        enemy.damage = data.damage;
        enemy.gold_value = data.BaseGoldValue;
    }
}

