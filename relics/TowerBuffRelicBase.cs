public abstract class TowerBuffRelicBase : Relic
{
    protected TowerBuffRelicBase(string id, bool isCursed = false) : base(id, isCursed)
    {
    }

    public override void OnObtain()
    {
        var towers = RunContextRuntime.TowersManager.GetTowers();
        for (int index = 0; index < towers.Count; index++)
        {
            TowerModel tower = towers[index];
            if (IsValidTower(tower))
            {
                AddBuff(tower);
            }
        }
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        if (IsValidTower(tower))
        {
            AddBuff(tower);
        }
    }

    public override void OnRemove()
    {
        var towers = RunContextRuntime.TowersManager.GetTowers();
        for (int index = 0; index < towers.Count; index++)
        {
            TowerModel tower = towers[index];
            if (IsValidTower(tower))
            {
                RemoveBuff(tower);
            }
        }
    }

    protected abstract void AddBuff(TowerModel tower);
    protected abstract void RemoveBuff(TowerModel tower);
    protected abstract bool IsValidTower(TowerModel tower);
}
