using System.Collections.Generic;

public sealed class TowersManagerRuntime
{
    private readonly List<TowerLogic> _towerListeners = new();

    public IReadOnlyList<TowerLogic> GetAllTowerListeners()
    {
        return this._towerListeners;
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
    }
}
