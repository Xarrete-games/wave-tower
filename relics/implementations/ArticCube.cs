public sealed class ArticCube : Relic
{
    private const float _extraDuration = 1f;

    public ArticCube() : base("artic_cube")
    {
    }

    public override void OnDebuffApplied(DebuffContext context, Enemy target)
    {
        if (context.Debuff.Type == EnemyDebuffModel.DebuffType.Frost)
        {
            context.Debuff.Duration += _extraDuration;
        }
    }
}
