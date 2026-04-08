public sealed class Salt : RelicModel
{
    private const float _lowHealthThreshold = 30f;
    private const float _bonusMultiplier = 0.3f;

    public Salt() : base("salt")
    {
    }

    public override void OnBeforeDamage(DamageContext context)
    {
        if (context.Target.GetPercentageRemainingHealth() <= _lowHealthThreshold)
        {
            context.ExtraMultiplicative += _bonusMultiplier;
        }
    }
}
