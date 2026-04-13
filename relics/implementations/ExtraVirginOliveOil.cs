public sealed class ExtraVirginOliveOil : Relic
{
    private const float _extraDuration = 1f;

    public ExtraVirginOliveOil() : base("extra_virgin_olive_oil")
    {
    }

    public override void OnDebuffApplied(DebuffContext context, EnemyModel target)
    {
        if (context.Debuff.Type == EnemyDebuffModel.DebuffType.Burn)
        {
            context.Debuff.Duration += _extraDuration;
        }
    }
}
