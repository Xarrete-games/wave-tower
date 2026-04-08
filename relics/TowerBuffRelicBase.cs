public abstract class TowerBuffRelicBase : RelicModel
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
            if (this.IsValidTower(tower))
            {
                this.AddBuff(tower);
            }
        }
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        if (this.IsValidTower(tower))
        {
            this.AddBuff(tower);
        }
    }

    public override void OnRemove()
    {
        var towers = RunContextRuntime.TowersManager.GetTowers();
        for (int index = 0; index < towers.Count; index++)
        {
            TowerModel tower = towers[index];
            if (this.IsValidTower(tower))
            {
                this.RemoveBuff(tower);
            }
        }
    }

    protected abstract void AddBuff(TowerModel tower);
    protected abstract void RemoveBuff(TowerModel tower);
    protected abstract bool IsValidTower(TowerModel tower);
}
