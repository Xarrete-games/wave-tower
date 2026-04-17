using Godot;

[GlobalClass]
public partial class WaveConfig : Resource
{
    [ExportGroup("Budget Scaling")]
    [Export]
    public int base_budget { get; set; } = 10;

    [Export]
    public int budget_per_wave { get; set; } = 5;

    [Export]
    public int exponential_start_wave { get; set; } = 8;

    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float exponential_growth { get; set; } = 0.08f;

    [ExportGroup("Spawn Intervals")]
    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_swarm_min { get; set; } = 0.5f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_swarm_max { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_speed_min { get; set; } = 0.7f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_speed_max { get; set; } = 1.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_normal_min { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_normal_max { get; set; } = 3.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_tank_min { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float spawn_interval_tank_max { get; set; } = 3.0f;

    [Export]
    public int spawn_interval_max_decay_every_waves { get; set; } = 5;

    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float spawn_interval_max_decay_amount { get; set; } = 0.02f;

    [Export(PropertyHint.Range, "0.01,2.0,0.01")]
    public float spawn_interval_min_cap { get; set; } = 0.1f;

    [Export]
    public float group_delay { get; set; } = 2.0f;

    [ExportGroup("Type Unlocks")]
    [Export]
    public int boss_wave_every { get; set; } = 10;

    [ExportGroup("Pressure Profile")]
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float primary_pressure_ratio { get; set; } = 0.7f;

    [Export]
    public Godot.Collections.Array<WaveTypeChance> wave_chances { get; set; } = new();

    public int BaseBudget
    {
        get => base_budget;
        set => base_budget = value;
    }

    public int BudgetPerWave
    {
        get => budget_per_wave;
        set => budget_per_wave = value;
    }

    public int ExponentialStartWave
    {
        get => exponential_start_wave;
        set => exponential_start_wave = value;
    }

    public float ExponentialGrowth
    {
        get => exponential_growth;
        set => exponential_growth = value;
    }

    public float SpawnIntervalSwarmMin
    {
        get => spawn_interval_swarm_min;
        set => spawn_interval_swarm_min = value;
    }

    public float SpawnIntervalSwarmMax
    {
        get => spawn_interval_swarm_max;
        set => spawn_interval_swarm_max = value;
    }

    public float SpawnIntervalSpeedMin
    {
        get => spawn_interval_speed_min;
        set => spawn_interval_speed_min = value;
    }

    public float SpawnIntervalSpeedMax
    {
        get => spawn_interval_speed_max;
        set => spawn_interval_speed_max = value;
    }

    public float SpawnIntervalNormalMin
    {
        get => spawn_interval_normal_min;
        set => spawn_interval_normal_min = value;
    }

    public float SpawnIntervalNormalMax
    {
        get => spawn_interval_normal_max;
        set => spawn_interval_normal_max = value;
    }

    public float SpawnIntervalTankMin
    {
        get => spawn_interval_tank_min;
        set => spawn_interval_tank_min = value;
    }

    public float SpawnIntervalTankMax
    {
        get => spawn_interval_tank_max;
        set => spawn_interval_tank_max = value;
    }

    public int SpawnIntervalMaxDecayEveryWaves
    {
        get => spawn_interval_max_decay_every_waves;
        set => spawn_interval_max_decay_every_waves = value;
    }

    public float SpawnIntervalMaxDecayAmount
    {
        get => spawn_interval_max_decay_amount;
        set => spawn_interval_max_decay_amount = value;
    }

    public float SpawnIntervalMinCap
    {
        get => spawn_interval_min_cap;
        set => spawn_interval_min_cap = value;
    }

    public float GroupDelay
    {
        get => group_delay;
        set => group_delay = value;
    }

    public int BossWaveEvery
    {
        get => boss_wave_every;
        set => boss_wave_every = value;
    }

    public float PrimaryPressureRatio
    {
        get => primary_pressure_ratio;
        set => primary_pressure_ratio = value;
    }

    public Godot.Collections.Array<WaveTypeChance> WaveChances
    {
        get => wave_chances;
        set => wave_chances = value;
    }
}
