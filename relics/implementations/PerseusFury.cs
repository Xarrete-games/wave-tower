public sealed class PerseusFury : TowerBuffRelicBase
{
    private const int _extraExecuteThreshold = 5;

    public PerseusFury() : base("perseus_fury")
    {
    }

    protected override void AddBuff(TowerModel tower)
    {
        tower.ExecuteThreshold += _extraExecuteThreshold;
    }

    protected override void RemoveBuff(TowerModel tower)
    {
        tower.ExecuteThreshold -= _extraExecuteThreshold;
    }

    protected override bool IsValidTower(TowerModel tower)
    {
        return tower.TypeId == "fire_laser_tower";
    }
}
