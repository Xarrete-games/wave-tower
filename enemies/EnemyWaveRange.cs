using Godot;

[GlobalClass]
public partial class EnemyWaveRange : Resource
{
    [Export]
    public int InitialWave { get; set; }

    [Export]
    public int FinalWave { get; set; }
}