using Godot;

[GlobalClass]
public partial class EnemyData : Resource
{
    public enum Type
    {
        SWARM,
        FAST,
        NORMAL,
        TANK,
        BOSS,
    }

    [ExportGroup("General")]
    [Export]
    public int type_legacy { get; set; }

    [Export]
    public Type type { get; set; } = Type.NORMAL;

    [Export]
    public string name { get; set; } = string.Empty;

    [Export(PropertyHint.MultilineText)]
    public string description { get; set; } = string.Empty;

    [Export]
    public Texture2D icon { get; set; }

    [ExportGroup("Scene")]
    [Export]
    public PackedScene scene { get; set; }

    [ExportGroup("Stats")]
    [Export]
    public int max_health { get; set; } = 50;

    [Export]
    public float base_speed { get; set; } = 80f;

    [Export]
    public int damage { get; set; } = 1;

    [Export]
    public int base_gold_value { get; set; } = 1;

    [ExportGroup("Wave")]
    [Export]
    public int weight { get; set; } = 1;

    [Export]
    public Godot.Collections.Array<EnemyWaveRange> available_waves { get; set; } = new();
}
