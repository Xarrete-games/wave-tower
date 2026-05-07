public sealed class HeadPhones : TowerBuffRelicBase
{
    private const float _extraChance = 0.2f;

    public HeadPhones() : base("headphones")
    {
    }

    protected override void AddBuff(TowerModel tower)
    {
        tower.DoubleShotChance += _extraChance;
    }

    protected override void RemoveBuff(TowerModel tower)
    {
        tower.DoubleShotChance -= _extraChance;
    }

    protected override bool IsValidTower(TowerModel tower)
    {
        return tower.TypeId == "frost_nova_tower";
    }
}
