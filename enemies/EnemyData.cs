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

    public int TypeLegacy
    {
        get => type_legacy;
        set => type_legacy = value;
    }

    public Type EnemyType
    {
        get => type;
        set => type = value;
    }

    public string EnemyName
    {
        get => name;
        set => name = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }

    public Texture2D Icon
    {
        get => icon;
        set => icon = value;
    }

    public PackedScene Scene
    {
        get => scene;
        set => scene = value;
    }

    public int MaxHealth
    {
        get => max_health;
        set => max_health = value;
    }

    public float BaseSpeed
    {
        get => base_speed;
        set => base_speed = value;
    }

    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    public int BaseGoldValue
    {
        get => base_gold_value;
        set => base_gold_value = value;
    }

    public int Weight
    {
        get => weight;
        set => weight = value;
    }

    public Godot.Collections.Array<EnemyWaveRange> AvailableWaves
    {
        get => available_waves;
        set => available_waves = value;
    }
}
