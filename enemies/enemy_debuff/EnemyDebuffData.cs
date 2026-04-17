using Godot;

[GlobalClass]
public partial class EnemyDebuffData : BaseData
{
    [ExportGroup("Debuff")]
    [Export]
    public int debuff_type { get; set; }

    [Export]
    public float value { get; set; }

    [Export]
    public float duration { get; set; }

    [Export]
    public float tick_interval { get; set; }

    [Export]
    public int max_stacks { get; set; } = 99;

    [ExportGroup("Script")]
    [Export]
    public Script runtime_script { get; set; }

    public int DebuffType
    {
        get => debuff_type;
        set => debuff_type = value;
    }

    public float Value
    {
        get => value;
        set => this.value = value;
    }

    public float Duration
    {
        get => duration;
        set => duration = value;
    }

    public float TickInterval
    {
        get => tick_interval;
        set => tick_interval = value;
    }

    public int MaxStacks
    {
        get => max_stacks;
        set => max_stacks = value;
    }

    public Script RuntimeScript
    {
        get => runtime_script;
        set => runtime_script = value;
    }

    public EnemyDebuff create_debuff()
    {
        return debuff_type switch
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
