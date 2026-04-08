public sealed class RunicLighter : RelicModel
{
    private const float _damagePerFireTower = 0.1f;

    public RunicLighter() : base("runic_lighter")
    {
    }

    public override void OnBeforeDamage(DamageContext context)
    {
        SourceModel source = context.Attack.Source;
        if (source.Type == SourceModel.SourceType.Debuff && source.TypeId == "burn_debuff")
        {
            int fireTowers = RunContextRuntime.TowersManager.GetTowerCount(TowerModel.TowerType.Fire);
            float extraMult = 1f + (_damagePerFireTower * fireTowers);
            context.ExtraMultiplicative += extraMult;
        }
    }
}
