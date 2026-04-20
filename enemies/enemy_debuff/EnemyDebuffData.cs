using Godot;

[GlobalClass]
public partial class EnemyDebuffData : BaseData
{
    [ExportGroup("Debuff")]
    [Export]
    public int DebuffType { get; set; }

    [Export]
    public float Value { get; set; }

    [Export]
    public float Duration { get; set; }

    [Export]
    public float TickInterval { get; set; }

    [Export]
    public int MaxStacks { get; set; } = 99;

    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public EnemyDebuff CreateDebuff()
    {
        return DebuffType switch
        {
            (int)EnemyDebuff.Type.Frost => new FrostDebuff(),
            (int)EnemyDebuff.Type.Burn => new BurnDebuff(),
            _ => null,
        };
    }

    public override Variant CreateItem()
    {
        // Debuffs are now created through CreateDebuff() typed path.
        return default;
    }
}
