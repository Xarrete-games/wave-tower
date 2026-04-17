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

    public EnemyDebuff create_debuff()
    {
        return debuff_type switch
        {
            (int)EnemyDebuff.Type.FROST => new FrostDebuff(),
            (int)EnemyDebuff.Type.BURN => new BurnDebuff(),
            _ => null,
        };
    }

    public override Variant create_item()
    {
        // Debuffs are now created through create_debuff() typed path.
        return default;
    }
}
