using System.Collections.Generic;

public sealed class TowersManagerRuntime
{
    private readonly System.Random _random = new();
    private readonly List<TowerLogic> _towerListeners = new();
    private readonly Dictionary<TowerModel.TowerType, int> _towerCounts = new();
    private readonly List<TowerModel> _towers = new();
    private readonly Dictionary<ulong, TowerModel> _towerByInstanceId = new();

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
        this.AddTowerPlaced(tower, 0UL);
    }

    public void AddTowerPlaced(TowerModel tower, ulong instanceId)
    {
        if (tower == null)
        {
            return;
        }

        this._towers.Add(tower);
        this._towerCounts[tower.Type] = this.GetTowerCount(tower.Type) + 1;

        if (instanceId != 0UL)
        {
            this._towerByInstanceId[instanceId] = tower;
        }
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

        this._towers.Remove(tower);
        this._towerCounts[tower.Type] = this.GetTowerCount(tower.Type) - 1;

        var keysToRemove = new List<ulong>();
        foreach (var pair in this._towerByInstanceId)
        {
            if (ReferenceEquals(pair.Value, tower))
            {
                keysToRemove.Add(pair.Key);
            }
        }

        for (int index = 0; index < keysToRemove.Count; index++)
        {
            this._towerByInstanceId.Remove(keysToRemove[index]);
        }
    }

    public bool RemoveTowerByInstanceId(ulong instanceId)
    {
        if (!this._towerByInstanceId.TryGetValue(instanceId, out TowerModel tower))
        {
            return false;
        }

        this._towerByInstanceId.Remove(instanceId);
        this.TowerRemoved(tower);
        return true;
    }

    public bool TryGetTowerByInstanceId(ulong instanceId, out TowerModel tower)
    {
        return this._towerByInstanceId.TryGetValue(instanceId, out tower);
    }

    public IReadOnlyList<TowerModel> GetTowers()
    {
        return this._towers;
    }

    public TowerModel PickRandomTower()
    {
        if (this._towers.Count == 0)
        {
            return null;
        }

        int index = this._random.Next(this._towers.Count);
        return this._towers[index];
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
        this._towers.Clear();
        this._towerByInstanceId.Clear();
    }
}
