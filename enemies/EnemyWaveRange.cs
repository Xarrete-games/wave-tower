using Godot;

[GlobalClass]
public partial class EnemyWaveRange : Resource
{
    [Export]
    public int initial_wave { get; set; }

    [Export]
    public int final_wave { get; set; }

    public int InitialWave
    {
        get => initial_wave;
        set => initial_wave = value;
    }

    public int FinalWave
    {
        get => final_wave;
        set => final_wave = value;
    }
}