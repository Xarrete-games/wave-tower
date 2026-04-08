using System.Collections.Generic;

public static class RunContextRuntime
{
    private static RelicsManagerRuntime _relicsManager = new RelicsManagerRuntime();
    private static TowersManagerRuntime _towersManager = new TowersManagerRuntime();

    public static RelicsManagerRuntime RelicsManager => _relicsManager;
    public static TowersManagerRuntime TowersManager => _towersManager;

    public static void Reset()
    {
        _relicsManager = new RelicsManagerRuntime();
        _towersManager = new TowersManagerRuntime();
    }

    public static List<AbstractModel> GetListeners()
    {
        var listeners = new List<AbstractModel>();

        foreach (AbstractModel relic in _relicsManager.GetAllRelicListeners())
        {
            listeners.Add(relic);
        }

        foreach (TowerLogic towerLogic in _towersManager.GetAllTowerListeners())
        {
            listeners.Add(towerLogic);
        }

        return listeners;
    }
}
