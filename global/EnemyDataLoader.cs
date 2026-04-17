using Godot;

public class EnemyDataLoader
{
    private const string DataPath = "res://enemies/data/";

    public Godot.Collections.Dictionary<int, EnemyData> enemies_data_dic = new();
    public Godot.Collections.Array<EnemyData> enemies_data = new();

    public EnemyDataLoader()
    {
        var resources = this.LoadResourcesFromDir(DataPath);
        for (int index = 0; index < resources.Count; index++)
        {
            Resource candidate = resources[index];
            EnemyData data = candidate as EnemyData;
            if (data == null)
            {
                GD.PushError($"[EnemyDataLoader] Resource is not of type EnemyData: {candidate}");
                continue;
            }

            this.enemies_data.Add(data);
        }

        for (int index = 0; index < this.enemies_data.Count; index++)
        {
            EnemyData enemyData = this.enemies_data[index];
            if (enemyData == null)
            {
                continue;
            }

            this.enemies_data_dic[enemyData.type_legacy] = enemyData;
        }
    }

    public Godot.Collections.Array<EnemyData> get_all_enemies_typed()
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int index = 0; index < this.enemies_data.Count; index++)
        {
            result.Add(this.enemies_data[index]);
        }

        return result;
    }

    public Godot.Collections.Array<Variant> get_all_enemies()
    {
        return ToVariantArray(this.get_all_enemies_typed());
    }

    public Godot.Collections.Array<EnemyData> get_enemies_by_type_typed(int type)
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int index = 0; index < this.enemies_data.Count; index++)
        {
            EnemyData data = this.enemies_data[index];
            if (data != null && (int)data.type == type)
            {
                result.Add(data);
            }
        }

        return result;
    }

    public Godot.Collections.Array<Variant> get_enemies_by_type(int type)
    {
        return ToVariantArray(this.get_enemies_by_type_typed(type));
    }

    public Godot.Collections.Array<EnemyData> get_spawnable_enemies_typed()
    {
        var result = new Godot.Collections.Array<EnemyData>();
        for (int index = 0; index < this.enemies_data.Count; index++)
        {
            EnemyData data = this.enemies_data[index];
            if (data != null && data.type != EnemyData.Type.BOSS)
            {
                result.Add(data);
            }
        }

        return result;
    }

    public Godot.Collections.Array<Variant> get_spawnable_enemies()
    {
        return ToVariantArray(this.get_spawnable_enemies_typed());
    }

    private Godot.Collections.Array<Resource> LoadResourcesFromDir(string path)
    {
        var result = new Godot.Collections.Array<Resource>();

        using DirAccess dir = DirAccess.Open(path);
        if (dir == null)
        {
            GD.PushError("[EnemyDataLoader] Directory not found: " + path);
            return result;
        }

        dir.ListDirBegin();
        string file = dir.GetNext();

        while (!string.IsNullOrEmpty(file))
        {
            if (file.EndsWith(".tres"))
            {
                Resource resource = GD.Load<Resource>(path + file);
                if (resource != null)
                {
                    result.Add(resource);
                }
            }

            file = dir.GetNext();
        }

        dir.ListDirEnd();
        return result;
    }

    private static Godot.Collections.Array<Variant> ToVariantArray(Godot.Collections.Array<EnemyData> source)
    {
        var result = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < source.Count; index++)
        {
            result.Add(Variant.From(source[index]));
        }

        return result;
    }
}
