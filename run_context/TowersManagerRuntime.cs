using System.Collections.Generic;

public sealed class TowersManagerRuntime
{
    private readonly List<TowerLogic> _towerListeners = new();
    private readonly Dictionary<TowerModel.TowerType, int> _towerCounts = new();

    public IReadOnlyList<TowerLogic> GetAllTowerListeners()
    {
        return this._towerListeners;
    }

    public IReadOnlyList<TowerLogic> GetTowerListeners()
    {
        return this.GetAllTowerListeners();
    }

    public void AddTowerPlaced(TowerLogic towerLogic)
    {
        this.AddTowerListener(towerLogic);
    }

    public void AddTowerPlaced(TowerModel tower)
    {
        if (tower == null)
        {
            return;
        }

        this._towerCounts[tower.Type] = this.GetTowerCount(tower.Type) + 1;
    }

    public void TowerRemoved(TowerLogic towerLogic)
    {
        this.RemoveTowerListener(towerLogic);
    }

    public void TowerRemoved(TowerModel tower)
    {
        if (tower == null)
        {
            return;
        }

        this._towerCounts[tower.Type] = this.GetTowerCount(tower.Type) - 1;
    }

    public int GetTowerCount(TowerModel.TowerType towerType)
    {
        if (!this._towerCounts.TryGetValue(towerType, out int count))
        {
            return 0;
        }

        return count;
    }

    public void AddTowerListener(TowerLogic towerLogic)
    {
        if (towerLogic == null)
        {
            return;
        }

        if (!this._towerListeners.Contains(towerLogic))
        {
            this._towerListeners.Add(towerLogic);
        }
    }

    public void RemoveTowerListener(TowerLogic towerLogic)
    {
        if (towerLogic == null)
        {
            return;
        }

        this._towerListeners.Remove(towerLogic);
    }

    public void Reset()
    {
        this._towerListeners.Clear();
        this._towerCounts.Clear();
    }
}
