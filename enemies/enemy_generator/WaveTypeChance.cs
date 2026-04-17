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

    public float ChanceFullSwarn
    {
        get => chance_full_swarn;
        set => chance_full_swarn = value;
    }

    public float ChanceFullSwarm
    {
        get => chance_full_swarn;
        set => chance_full_swarn = value;
    }

    public float ChanceFullSpeed
    {
        get => chance_full_speed;
        set => chance_full_speed = value;
    }

    public float ChanceFullTank
    {
        get => chance_full_tank;
        set => chance_full_tank = value;
    }
}
