using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class WaveSpawner : Node
{
    [Signal]
    public delegate void wave_startedEventHandler(int wave_number);

    [Signal]
    public delegate void enemy_spawnedEventHandler(Variant enemy);

    [Signal]
    public delegate void group_finishedEventHandler(int group_index, int pressure);

    [Signal]
    public delegate void wave_finishedEventHandler(int wave_number);

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
        if (this._is_spawning)
        {
            GD.PushWarning("[WaveSpawner] Already spawning a wave - ignoring request");
            return;
        }

        if (this.world_map == null)
        {
            GD.PushWarning("[WaveSpawner] No world_map assigned");
            return;
        }

        if (config == null)
        {
            GD.PushWarning("[WaveSpawner] No WaveConfig provided");
            return;
        }

        this._is_spawning = true;
        EmitSignal(SignalName.wave_started, wave_number);

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
                this._spawn_single(enemies[enemyIndex]);

                if (enemyIndex < enemies.Count - 1)
                {
                    float spawnInterval = this._pick_spawn_interval(pressure, wave_number, config);
                    await ToSignal(GetTree().CreateTimer(spawnInterval, false), SceneTreeTimer.SignalName.Timeout);
                }
            }

            EmitSignal(SignalName.group_finished, groupIndex, pressure);

            if (groupIndex < groups.Count - 1)
            {
                float groupDelay = config.group_delay;
                await ToSignal(GetTree().CreateTimer(groupDelay, false), SceneTreeTimer.SignalName.Timeout);
            }
        }

        this._is_spawning = false;
        EmitSignal(SignalName.wave_finished, wave_number);
    }

    private float _pick_spawn_interval(int pressure, int wave_number, WaveConfig config)
    {
        Vector2 intervalRange = this._get_spawn_interval_range(pressure, config);
        Vector2 decayedRange = this._get_decayed_spawn_interval_range(intervalRange, wave_number, config);
        float minSpawnInterval = this._get_min_spawn_interval(config);
        float minInterval = Mathf.Max(decayedRange.X, minSpawnInterval);
        float maxInterval = Mathf.Max(decayedRange.Y, minInterval);
        maxInterval = Mathf.Max(maxInterval, minInterval);

        if (Mathf.IsEqualApprox(minInterval, maxInterval))
        {
            return minInterval;
        }

        return (float)GD.RandRange(minInterval, maxInterval);
    }

    private Vector2 _get_spawn_interval_range(int pressure, WaveConfig config)
    {
        return pressure switch
        {
            0 => new Vector2(config.spawn_interval_swarm_min, config.spawn_interval_swarm_max),
            1 => new Vector2(config.spawn_interval_speed_min, config.spawn_interval_speed_max),
            2 => new Vector2(config.spawn_interval_tank_min, config.spawn_interval_tank_max),
            _ => new Vector2(config.spawn_interval_normal_min, config.spawn_interval_normal_max),
        };
    }

    private Vector2 _get_decayed_spawn_interval_range(Vector2 base_range, int wave_number, WaveConfig config)
    {
        int everyWaves = Mathf.Max(config.spawn_interval_max_decay_every_waves, 1);
        int decaySteps = Mathf.Max((wave_number - 1) / everyWaves, 0);
        float decayAmount = decaySteps * config.spawn_interval_max_decay_amount;
        float minSpawnInterval = this._get_min_spawn_interval(config);

        float decayedMin = Mathf.Max(base_range.X - decayAmount, minSpawnInterval);
        float decayedMax = Mathf.Max(base_range.Y - decayAmount, minSpawnInterval);
        if (decayedMax < decayedMin)
        {
            decayedMax = decayedMin;
        }

        return new Vector2(decayedMin, decayedMax);
    }

    private float _get_min_spawn_interval(WaveConfig config)
    {
        return Mathf.Max(config.spawn_interval_min_cap, 0.01f);
    }

    private void _spawn_single(EnemyData data)
    {
        if (data == null)
        {
            return;
        }

        Godot.Collections.Array<Godot.Collections.Dictionary> portalEntries = this.world_map.portal_entries;
        if (portalEntries.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No spawn points available");
            return;
        }

        int portalIndex = (int)(GD.Randi() % (uint)portalEntries.Count);
        Godot.Collections.Dictionary spawnEntry = portalEntries[portalIndex];
        Godot.Collections.Array<Vector2> waypoints = this.world_map.get_waypoints_for_spawn(spawnEntry);
        if (waypoints.Count == 0)
        {
            GD.PushWarning("[WaveSpawner] No waypoints for spawn entry");
            return;
        }

        Godot.Collections.Array<Vector2> enemyWaypoints = this._build_enemy_waypoints_with_offset(waypoints);
        PackedScene scene = data.scene ?? this.fallback_enemy_scene;
        if (scene == null)
        {
            GD.PushError($"[WaveSpawner] No scene for enemy '{data.name}' and no fallback set");
            return;
        }

        Node enemy = scene.Instantiate();
        if (enemy == null)
        {
            GD.PushError($"[WaveSpawner] Scene for '{data.name}' did not produce an Enemy");
            return;
        }

        this._apply_stats(enemy, data);
        enemy.AddToGroup("enemy");
        enemy.Set("enabled", false);

        if (this.enemies_container == null)
        {
            GD.PushWarning("[WaveSpawner] enemies_container is null");
            enemy.QueueFree();
            return;
        }

        this.enemies_container.AddChild(enemy);
        enemy.Call("disable");
        enemy.Set("global_position", enemyWaypoints[0]);
        enemy.Call("enable");
        enemy.Call("set_waypoints", enemyWaypoints);

        EmitSignal(SignalName.enemy_spawned, enemy);
    }

    private Godot.Collections.Array<Vector2> _build_enemy_waypoints_with_offset(Godot.Collections.Array<Vector2> base_waypoints)
    {
        if (base_waypoints.Count == 0)
        {
            return new Godot.Collections.Array<Vector2>();
        }

        float minY = Mathf.Min(this.path_offset_y_min, this.path_offset_y_max);
        float maxY = Mathf.Max(this.path_offset_y_min, this.path_offset_y_max);
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

    private void _apply_stats(Node enemy, EnemyData data)
    {
        enemy.Set("max_health", data.max_health);
        enemy.Set("base_speed", data.base_speed);
        enemy.Set("damage", data.damage);
        enemy.Set("gold_value", data.base_gold_value);
    }
}
