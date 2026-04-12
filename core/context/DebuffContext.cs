using Godot;

[GlobalClass]
public partial class DebuffContext : RefCounted
{
    public Variant debuff { get; set; }
    public int stacks { get; set; }

    public EnemyDebuffModel Debuff { get; }
    public int Stacks { get => this.stacks; set => this.stacks = value; }

    public DebuffContext(EnemyDebuffModel debuff, int stacks)
    {
        this.Debuff = debuff;
        this.Stacks = stacks;
    }

    public DebuffContext(Variant p_debuff, int p_stacks)
    {
        this.debuff = p_debuff;
        this.stacks = p_stacks;
    }

    public DebuffContext(EnemyDebuff p_debuff, int p_stacks)
    {
        this.debuff = p_debuff;
        this.stacks = p_stacks;
    }
}
