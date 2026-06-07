using System.Collections.Generic;

public static class RunContextRuntime
{
    private static RelicsManagerRuntime _relicsManager = new RelicsManagerRuntime();
    private static TowersManagerRuntime _towersManager = new TowersManagerRuntime();
    private static ConsumablesManagerRuntime _consumablesManager = new ConsumablesManagerRuntime();
    private static EconomyRuntime _economy = new EconomyRuntime();
    private static RunContextRuntimeModels.Status _status = new RunContextRuntimeModels.Status();
    private static CompositeTileMapRuntime _compositeTileMap = new CompositeTileMapRuntime();

    public static RelicsManagerRuntime RelicsManager => _relicsManager;
    public static TowersManagerRuntime TowersManager => _towersManager;
    public static ConsumablesManagerRuntime ConsumablesManager => _consumablesManager;
    public static EconomyRuntime Economy => _economy;
    public static RunContextRuntimeModels.Status Status => _status;
    public static CompositeTileMapRuntime CompositeTileMap => _compositeTileMap;

    public static void Reset()
    {
        _relicsManager = new RelicsManagerRuntime();
        _towersManager = new TowersManagerRuntime();
        _consumablesManager = new ConsumablesManagerRuntime();
        _economy = new EconomyRuntime();
        _status = new RunContextRuntimeModels.Status();
        _compositeTileMap = new CompositeTileMapRuntime();
    }

    public static List<AbstractModel> GetListeners()
    {
        var listeners = new List<AbstractModel>();

        foreach (AbstractModel relic in _relicsManager.GetAllRelicListeners())
        {
            listeners.Add(relic);
        }

        foreach (Tower tower in _towersManager.GetAllTowerListeners())
        {
            listeners.Add(tower);
        }

        return listeners;
    }
}
