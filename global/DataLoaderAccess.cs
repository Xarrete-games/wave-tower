using Godot;
using System.Collections.Generic;

public static class DataLoaderAccess
{
    public static Godot.Collections.Array<Variant> GetAllRelics()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return dataLoader.get_all_relics();
    }

    public static Godot.Collections.Array<Variant> GetAllConsumables()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return dataLoader.get_all_consumables();
    }

    public static List<RelicData> GetAllRelicsTyped()
    {
        var result = new List<RelicData>();
        Godot.Collections.Array<Variant> allRelics = GetAllRelics();
        for (int index = 0; index < allRelics.Count; index++)
        {
            RelicData relicData = allRelics[index].AsGodotObject() as RelicData;
            if (relicData != null)
            {
                result.Add(relicData);
            }
        }

        return result;
    }

    public static List<ConsumableData> GetAllConsumablesTyped()
    {
        var result = new List<ConsumableData>();
        Godot.Collections.Array<Variant> allConsumables = GetAllConsumables();
        for (int index = 0; index < allConsumables.Count; index++)
        {
            ConsumableData consumableData = allConsumables[index].AsGodotObject() as ConsumableData;
            if (consumableData != null)
            {
                result.Add(consumableData);
            }
        }

        return result;
    }

    public static RelicData GetRelicById(string relicId)
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        Variant data = dataLoader?.get_relic_by_id(relicId) ?? default;
        return data.AsGodotObject() as RelicData;
    }

    public static List<RelicData> GetRandomRelicsTyped(int amount)
    {
        var result = new List<RelicData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        Godot.Collections.Array<Variant> randomRelics = dataLoader?.get_random_relics(amount) ?? new Godot.Collections.Array<Variant>();
        for (int index = 0; index < randomRelics.Count; index++)
        {
            RelicData relicData = randomRelics[index].AsGodotObject() as RelicData;
            if (relicData != null)
            {
                result.Add(relicData);
            }
        }

        return result;
    }

    public static ConsumableData GetConsumableById(string consumableId)
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        Variant data = dataLoader?.get_consumable_by_id(consumableId) ?? default;
        return data.AsGodotObject() as ConsumableData;
    }

    public static List<ConsumableData> GetAllConsumablesByTypeTyped(int consumableType)
    {
        var result = new List<ConsumableData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        Godot.Collections.Array<Variant> items = dataLoader?.get_all_consumables_of_type(consumableType) ?? new Godot.Collections.Array<Variant>();
        for (int index = 0; index < items.Count; index++)
        {
            ConsumableData consumableData = items[index].AsGodotObject() as ConsumableData;
            if (consumableData != null)
            {
                result.Add(consumableData);
            }
        }

        return result;
    }

    public static List<RelicData> GetNotUsedRelicsTyped(int? rarity = null, bool? isCursed = null)
    {
        var result = new List<RelicData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        Variant rarityVariant = rarity.HasValue ? Variant.From(rarity.Value) : default;
        Variant cursedVariant = isCursed.HasValue ? Variant.From(isCursed.Value) : default;
        Godot.Collections.Array<Variant> items = dataLoader?.get_not_used_relics(rarityVariant, cursedVariant) ?? new Godot.Collections.Array<Variant>();
        for (int index = 0; index < items.Count; index++)
        {
            RelicData relicData = items[index].AsGodotObject() as RelicData;
            if (relicData != null)
            {
                result.Add(relicData);
            }
        }

        return result;
    }

    public static List<EventData> GetAllEventsTyped()
    {
        var result = new List<EventData>();
        DataLoader dataLoader = GetDataLoaderSingleton();
        Godot.Collections.Array<Variant> allEvents = dataLoader?.get_all_events() ?? new Godot.Collections.Array<Variant>();
        for (int index = 0; index < allEvents.Count; index++)
        {
            EventData eventData = allEvents[index].AsGodotObject() as EventData;
            if (eventData != null)
            {
                result.Add(eventData);
            }
        }

        return result;
    }

    public static Godot.Collections.Array<Variant> GetAllTowerData()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return dataLoader.get_all_tower_data();
    }

    public static Godot.Collections.Array<Variant> GetAllEnemies()
    {
        DataLoader dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return dataLoader.get_all_enemies();
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