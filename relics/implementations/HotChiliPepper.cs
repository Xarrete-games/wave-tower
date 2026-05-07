public sealed class HotChiliPepper : TowerBuffRelicBase
{
    public HotChiliPepper() : base("hot_chili_pepper")
    {
    }

    protected override void AddBuff(TowerModel tower)
    {
        tower.ApplyBurn = true;
    }

    protected override void RemoveBuff(TowerModel tower)
    {
        tower.ApplyBurn = false;
    }

    protected override bool IsValidTower(TowerModel tower)
    {
        return tower.Type == TowerModel.TowerType.Fire && tower.TypeId != "wild_fire_tower";
    }
}
