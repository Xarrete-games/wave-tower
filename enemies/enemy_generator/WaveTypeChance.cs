using Godot;

[GlobalClass]
public partial class WaveTypeChance : Resource
{
    [Export]
    public float chance_full_swarn { get; set; } = 0;

    [Export]
    public float chance_full_speed { get; set; } = 0;

    [Export]
    public float chance_full_tank { get; set; } = 0;
}
