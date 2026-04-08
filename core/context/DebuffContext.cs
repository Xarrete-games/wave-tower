public sealed class DebuffContext
{
    public EnemyDebuffModel Debuff { get; }
    public int Stacks { get; set; }

    public DebuffContext(EnemyDebuffModel debuff, int stacks)
    {
        this.Debuff = debuff;
        this.Stacks = stacks;
    }
}
