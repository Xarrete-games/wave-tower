public sealed class DonRafaelPipe : Relic
{
    private const float _extraDuration = 1f;

    public DonRafaelPipe() : base("don_rafael_pipe")
    {
    }

    public override void OnDebuffApplied(DebuffContext context, EnemyModel target)
    {
        if (context.Debuff.Type == EnemyDebuffModel.DebuffType.Frost || context.Debuff.Type == EnemyDebuffModel.DebuffType.Burn)
        {
            context.Debuff.Duration += _extraDuration;
        }
    }
}
