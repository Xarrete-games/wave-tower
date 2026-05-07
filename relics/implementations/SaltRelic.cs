public sealed class SaltRelic : Relic
{
    private const float LOW_HEALTH_THRESHOLD = 30f;
    private const float BONUS_MULTIPLIER = 0.3f;

    public SaltRelic() : base("salt")
    {
    }

    public override void OnBeforeDamage(DamageContext ctx)
    {
        if (ctx == null || ctx.Target == null)
        {
            return;
        }

        float healthPercent = ctx.Target.GetPercentageRemainingHealth();
        if (healthPercent > LOW_HEALTH_THRESHOLD)
        {
            return;
        }

        ctx.ExtraMultiplicative += BONUS_MULTIPLIER;
    }
}
