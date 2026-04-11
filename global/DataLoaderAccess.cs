using Godot;

public static class DataLoaderAccess
{
    public static Godot.Collections.Array<Variant> GetAllTowerData()
    {
        Node dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return dataLoader.Call("get_all_tower_data").AsGodotArray<Variant>();
    }

    public static Godot.Collections.Array<Variant> GetAllEnemies()
    {
        Node dataLoader = GetDataLoaderSingleton();
        if (dataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        GodotObject enemyDataLoader = dataLoader.Get("enemy_data").AsGodotObject();
        if (enemyDataLoader == null)
        {
            return new Godot.Collections.Array<Variant>();
        }

        return enemyDataLoader.Call("get_all_enemies").AsGodotArray<Variant>();
    }

    private static Node GetDataLoaderSingleton()
    {
        if (DataLoader.Instance != null)
        {
            return DataLoader.Instance;
        }

        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        return tree?.Root?.GetNodeOrNull<Node>("DataLoader");
    }
}