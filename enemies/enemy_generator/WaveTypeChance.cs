using Godot;

[GlobalClass]
public partial class WaveTypeChance : Resource
{
    [Export]
    public float ChanceFullSwarm { get; set; } = 0;

    [Export]
    public float ChanceFullSpeed { get; set; } = 0;

    [Export]
    public float ChanceFullTank { get; set; } = 0;
}
