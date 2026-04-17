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

    // Legacy compatibility aliases (non-exported).
    public float value
    {
        get => Value;
        set => Value = value;
    }

    public float duration
    {
        get => Duration;
        set => Duration = value;
    }

    [Export]
    public float TickInterval { get; set; }

    [Export]
    public int MaxStacks { get; set; } = 99;

    [ExportGroup("Script")]
    [Export]
    public Script RuntimeScript { get; set; }

    public EnemyDebuff create_debuff()
    {
        return DebuffType switch
        {
            (int)EnemyDebuff.Type.FROST => new FrostDebuff(),
            (int)EnemyDebuff.Type.BURN => new BurnDebuff(),
            _ => null,
        };
    }

    public EnemyDebuff CreateDebuff()
    {
        return create_debuff();
    }

    public override Variant create_item()
    {
        // Debuffs are now created through create_debuff() typed path.
        return default;
    }
}
