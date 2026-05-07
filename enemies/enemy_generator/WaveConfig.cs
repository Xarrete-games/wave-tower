using Godot;
using System;

[GlobalClass]
public partial class WaveConfig : Resource
{
    [ExportGroup("Budget Scaling")]
    [Export]
    public int BaseBudget { get; set; } = 10;

    [Export]
    public int BudgetPerWave { get; set; } = 5;

    [Export]
    public int ExponentialStartWave { get; set; } = 8;

    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float ExponentialGrowth { get; set; } = 0.08f;

    [ExportGroup("Spawn Intervals")]
    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalSwarmMin { get; set; } = 0.5f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalSwarmMax { get; set; } = 0.8f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalSpeedMin { get; set; } = 0.7f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalSpeedMax { get; set; } = 1.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalNormalMin { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalNormalMax { get; set; } = 3.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalTankMin { get; set; } = 2.0f;

    [Export(PropertyHint.Range, "0.1,10.0,0.01")]
    public float SpawnIntervalTankMax { get; set; } = 3.0f;

    [Export]
    public int SpawnIntervalMaxDecayEveryWaves { get; set; } = 5;

    [Export(PropertyHint.Range, "0.0,1.0,0.01")]
    public float SpawnIntervalMaxDecayAmount { get; set; } = 0.02f;

    [Export(PropertyHint.Range, "0.01,2.0,0.01")]
    public float SpawnIntervalMinCap { get; set; } = 0.1f;

    [Export]
    public float GroupDelay { get; set; } = 2.0f;

    [ExportGroup("Type Unlocks")]
    [Export]
    public int BossWaveEvery { get; set; } = 10;

    [ExportGroup("Pressure Profile")]
    [Export(PropertyHint.Range, "0.0,1.0")]
    public float PrimaryPressureRatio { get; set; } = 0.7f;

    [Export]
    public WaveTypeChance[] WaveChances { get; set; } = Array.Empty<WaveTypeChance>();
}
