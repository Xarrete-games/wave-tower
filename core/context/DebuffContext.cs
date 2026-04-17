public class DebuffContext
{
    public int stacks { get; set; }

    public EnemyDebuffModel Debuff { get; }
    public int Stacks { get => stacks; set => stacks = value; }

    public DebuffContext(EnemyDebuffModel debuff, int stacks)
    {
        Debuff = debuff;
        Stacks = stacks;
    }
}
