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
        return _towerListeners;
    }

    public IReadOnlyList<TowerLogic> GetTowerListeners()
    {
        return GetAllTowerListeners();
    }

    public void AddTowerPlaced(TowerLogic towerLogic)
    {
        AddTowerListener(towerLogic);
    }

    public void AddTowerPlaced(TowerModel tower)
    {
        AddTowerPlaced(tower, 0UL);
    }

    public void AddTowerPlaced(TowerModel tower, ulong instanceId)
    {
        if (tower == null)
        {
            return;
        }

        _towers.Add(tower);
        _towerCounts[tower.Type] = GetTowerCount(tower.Type) + 1;

        if (instanceId != 0UL)
        {
            _towerByInstanceId[instanceId] = tower;
        }
    }

    public void TowerRemoved(TowerLogic towerLogic)
    {
        RemoveTowerListener(towerLogic);
    }

    public void TowerRemoved(TowerModel tower)
    {
        if (tower == null)
        {
            return;
        }

        _towers.Remove(tower);
        _towerCounts[tower.Type] = GetTowerCount(tower.Type) - 1;

        var keysToRemove = new List<ulong>();
        foreach (var pair in _towerByInstanceId)
        {
            if (ReferenceEquals(pair.Value, tower))
            {
                keysToRemove.Add(pair.Key);
            }
        }

        for (int index = 0; index < keysToRemove.Count; index++)
        {
            _towerByInstanceId.Remove(keysToRemove[index]);
        }
    }

    public bool RemoveTowerByInstanceId(ulong instanceId)
    {
        if (!_towerByInstanceId.TryGetValue(instanceId, out TowerModel tower))
        {
            return false;
        }

        _towerByInstanceId.Remove(instanceId);
        TowerRemoved(tower);
        return true;
    }

    public bool TryGetTowerByInstanceId(ulong instanceId, out TowerModel tower)
    {
        return _towerByInstanceId.TryGetValue(instanceId, out tower);
    }

    public IReadOnlyList<TowerModel> GetTowers()
    {
        return _towers;
    }

    public TowerModel PickRandomTower()
    {
        if (_towers.Count == 0)
        {
            return null;
        }

        int index = _random.Next(_towers.Count);
        return _towers[index];
    }

    public int GetTowerCount(TowerModel.TowerType towerType)
    {
        if (!_towerCounts.TryGetValue(towerType, out int count))
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

        if (!_towerListeners.Contains(towerLogic))
        {
            _towerListeners.Add(towerLogic);
        }
    }

    public void RemoveTowerListener(TowerLogic towerLogic)
    {
        if (towerLogic == null)
        {
            return;
        }

        _towerListeners.Remove(towerLogic);
    }

    public void Reset()
    {
        _towerListeners.Clear();
        _towerCounts.Clear();
        _towers.Clear();
        _towerByInstanceId.Clear();
    }
}
