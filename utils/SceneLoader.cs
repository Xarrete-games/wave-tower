using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SceneLoader : Node
{
    private static Array<string> GetSceneFiles(string path)
    {
        DirAccess dir = DirAccess.Open(path);
        if (dir == null)
        {
            GD.PushError("[SceneLoader]: Invalid directory path: " + path + ".");
            return new Array<string>();
        }

        Array<string> sceneFiles = new();
        dir.ListDirBegin();

        while (true)
        {
            string fileName = dir.GetNext();
            if (string.IsNullOrEmpty(fileName))
            {
                break;
            }

            if (!dir.CurrentIsDir() && fileName.EndsWith(".tscn"))
            {
                sceneFiles.Add(fileName);
            }
        }

        dir.ListDirEnd();

        if (sceneFiles.Count == 0)
        {
            GD.PushError("[SceneLoader]: No .tscn files found in " + path);
        }

        return sceneFiles;
    }

    public static PackedScene GetRandomSceneFromPath(string path)
    {
        Array<string> sceneFiles = GetSceneFiles(path);
        if (sceneFiles.Count == 0)
        {
            return null;
        }

        int randomIndex = (int)GD.RandRange(0, sceneFiles.Count - 1);
        string fullPath = path.PathJoin(sceneFiles[randomIndex]);

        PackedScene loadedScene = ResourceLoader.Load<PackedScene>(fullPath);
        if (loadedScene != null)
        {
            return loadedScene;
        }

        GD.PushError("[SceneLoader]: Resource is not a PackedScene: " + fullPath);
        return null;
    }

    public static PackedScene GetIndexedSceneFromPath(string path, int index)
    {
        Array<string> sceneFiles = GetSceneFiles(path);
        if (sceneFiles.Count == 0)
        {
            return null;
        }

        if (index < 0 || index >= sceneFiles.Count)
        {
            GD.PushError("[SceneLoader]: Index " + index + " is out of bounds (0 to " + (sceneFiles.Count - 1) + ") for path: " + path);
            return null;
        }

        string fullPath = path.PathJoin(sceneFiles[index]);
        PackedScene loadedScene = ResourceLoader.Load<PackedScene>(fullPath);
        if (loadedScene != null)
        {
            return loadedScene;
        }

        GD.PushError("[SceneLoader]: Resource is not a PackedScene: " + fullPath);
        return null;
    }
}
