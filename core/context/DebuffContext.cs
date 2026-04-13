public class DebuffContext
{
    public int stacks { get; set; }

    public EnemyDebuffModel Debuff { get; }
    public int Stacks { get => this.stacks; set => this.stacks = value; }

    public DebuffContext(EnemyDebuffModel debuff, int stacks)
    {
        this.Debuff = debuff;
        this.Stacks = stacks;
    }
}
