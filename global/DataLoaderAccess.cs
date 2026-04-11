using Godot;

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