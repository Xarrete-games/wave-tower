using Godot;
using System.Collections.Generic;

public static class DataLoaderAccess
{
    public static List<RelicData> GetAllRelics()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new List<RelicData>();
        }

        return dataLoader.GetAllRelics();
    }

    public static List<ConsumableData> GetAllConsumables()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new List<ConsumableData>();
        }

        return dataLoader.GetAllConsumables();
    }

    public static List<RelicData> GetAllRelicsTyped()
    {
        return GetAllRelics();
    }

    public static List<ConsumableData> GetAllConsumablesTyped()
    {
        return GetAllConsumables();
    }

    public static RelicData GetRelicById(string relicId)
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetRelicById(relicId);
    }

    public static List<RelicData> GetRandomRelicsTyped(int amount)
    {
        var result = new List<RelicData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetRandomRelics(amount) ?? result;
    }

    public static ConsumableData GetConsumableById(string consumableId)
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetConsumableById(consumableId);
    }

    public static List<ConsumableData> GetAllConsumablesByTypeTyped(int consumableType)
    {
        var result = new List<ConsumableData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetAllConsumablesOfType(consumableType) ?? result;
    }

    public static List<RelicData> GetNotUsedRelicsTyped(int? rarity = null, bool? isCursed = null)
    {
        var result = new List<RelicData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetNotUsedRelics(rarity, isCursed) ?? result;
    }

    public static List<EventData> GetAllEventsTyped()
    {
        var result = new List<EventData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        return dataLoader?.GetAllEvents() ?? result;
    }

    public static List<TowerDataWithInstance> GetAllTowerData()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new List<TowerDataWithInstance>();
        }

        return dataLoader.GetAllTowerData();
    }

    public static List<TowerDataWithInstance> GetAllTowerDataTyped()
    {
        return GetAllTowerData();
    }

    public static List<EnemyData> GetAllEnemies()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new List<EnemyData>();
        }

        return dataLoader.GetAllEnemies();
    }

    private static DataLoader GetDataLoaderSingleton()
    {
        if (DataLoader.Instance != null)
        {
            return DataLoader.Instance;
        }

        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        return tree?.Root?.GetNodeOrNull<DataLoader>("DataLoader");
    }
}
