using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    public enum EnemyType
    {
        SWARM,
        FAST,
        NORMAL,
        TANK,
        BOSS,
    }

    [ExportGroup("General")]
    [Export]
    public EnemyType Type { get; set; } = EnemyType.NORMAL;

    [Export]
    public string Name { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = string.Empty;

    [Export]
    public Texture2D Icon { get; set; }

    [ExportGroup("Scene")]
    [Export]
    public PackedScene Scene { get; set; }

    [ExportGroup("Stats")]
    [Export]
    public int MaxHealth { get; set; } = 50;

    [Export]
    public float BaseSpeed { get; set; } = 80f;

    [Export]
    public int Damage { get; set; } = 1;

    [Export]
    public int BaseGoldValue { get; set; } = 1;

    [ExportGroup("Wave")]
    [Export]
    public int Weight { get; set; } = 1;

    [Export]
    public Godot.Collections.Array<EnemyWaveRange> AvailableWaves { get; set; } = new();

    public EnemyType EnemyTypeValue
    {
        get => Type;
        set => Type = value;
    }

}
