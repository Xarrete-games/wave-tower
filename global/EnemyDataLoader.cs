using Godot;
using System.Collections.Generic;

public class EnemyDataLoader
{
    private const string DataPath = "res://enemies/data/";

    public Dictionary<int, EnemyData> EnemiesByLegacyType { get; } = new();
    private readonly List<EnemyData> _enemies = new();

    public EnemyDataLoader()
    {
        var resources = LoadResourcesFromDir(DataPath);
        for (int index = 0; index < resources.Count; index++)
        {
            Resource candidate = resources[index];
            EnemyData data = candidate as EnemyData;
            if (data == null)
            {
                GD.PushError($"[EnemyDataLoader] Resource is not of type EnemyData: {candidate}");
                continue;
            }

            _enemies.Add(data);
        }

        for (int index = 0; index < _enemies.Count; index++)
        {
            EnemyData enemyData = _enemies[index];
            if (enemyData == null)
            {
                continue;
            }
        }
    }

    public List<EnemyData> GetAllEnemies()
    {
        var result = new List<EnemyData>(_enemies.Count);
        for (int index = 0; index < _enemies.Count; index++)
        {
            result.Add(DuplicateResource(_enemies[index]));
        }

        return result;
    }

    public List<EnemyData> GetEnemiesByType(int type)
    {
        var result = new List<EnemyData>();
        for (int index = 0; index < _enemies.Count; index++)
        {
            EnemyData data = _enemies[index];
            if (data != null && (int)data.Type == type)
            {
                result.Add(DuplicateResource(data));
            }
        }

        return result;
    }

    public List<EnemyData> GetSpawnableEnemies()
    {
        var result = new List<EnemyData>();
        for (int index = 0; index < _enemies.Count; index++)
        {
            EnemyData data = _enemies[index];
            if (data != null && data.Type != EnemyData.EnemyType.BOSS)
            {
                result.Add(DuplicateResource(data));
            }
        }

        return result;
    }

    private List<Resource> LoadResourcesFromDir(string path)
    {
        var result = new List<Resource>();

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

    private static EnemyData DuplicateResource(EnemyData resource)
    {
        if (resource == null)
        {
            return null;
        }

        return resource.Duplicate(true) as EnemyData;
    }
}


