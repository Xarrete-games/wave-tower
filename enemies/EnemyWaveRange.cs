using Godot;

[GlobalClass]
public partial class EnemyWaveRange : Resource
{
    [Export]
    public int initial_wave { get; set; }

    [Export]
    public int final_wave { get; set; }
}