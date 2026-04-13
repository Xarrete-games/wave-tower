public sealed class BlackWitchHat : Relic
{
    private const float _bonusDamage = 5f;

    public BlackWitchHat() : base("black_witch_hat")
    {
    }

    public override void OnBeforeDamage(DamageContext context)
    {
        if (context.Target.HasAnyDebuff && context.Attack.Source.Type == SourceModel.SourceType.Tower)
        {
            context.ExtraAdditive += _bonusDamage;
        }
    }
}
