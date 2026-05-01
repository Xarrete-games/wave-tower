public class DebuffContext
{
    private int _stacks;

    public EnemyDebuffModel Debuff { get; }
    public int Stacks { get => _stacks; set => _stacks = value; }

    public DebuffContext(EnemyDebuffModel debuff, int stacks)
    {
        Debuff = debuff;
        Stacks = stacks;
    }
}
