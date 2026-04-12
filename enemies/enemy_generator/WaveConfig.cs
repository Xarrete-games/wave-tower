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
}
