using System;
using Godot;

[GlobalClass]
public partial class MapPieceData : Resource
{
    private const string DefaultSceneDirectory = "res://levels/map_pieces/instances/";
    private const string InitialSceneDirectory = "res://levels/map_pieces/init_maps/";
    private const string SceneExtension = ".tscn";

    [Export]
    public Edge[] Edges { get; set; } = Array.Empty<Edge>();

    [Export]
    public string SceneId { get; set; } = string.Empty;

    [Export]
    public bool IsInitialPiece { get; set; }

    private PackedScene _cachedScene;

    public bool IsFork
    {
        get => Edges != null && Edges.Length > 2;
    }

    public bool HasEdge(Edge edge)
    {
        if (edge == null || Edges == null)
        {
            return false;
        }

        for (int i = 0; i < Edges.Length; i++)
        {
            Edge current = Edges[i];
            if (current != null && current.Matches(edge))
            {
                return true;
            }
        }

        return false;
    }

    public bool HasConnectingEdge(Edge edge)
    {
        if (edge == null || Edges == null)
        {
            return false;
        }

        for (int i = 0; i < Edges.Length; i++)
        {
            Edge current = Edges[i];
            if (current != null && current.CanConnectWith(edge))
            {
                return true;
            }
        }

        return false;
    }

    public Edge GetConnectingEdge(Edge edge)
    {
        if (edge == null || Edges == null)
        {
            return null;
        }

        for (int i = 0; i < Edges.Length; i++)
        {
            Edge current = Edges[i];
            if (current != null && current.CanConnectWith(edge))
            {
                return current;
            }
        }

        return null;
    }

    public bool HasEdgeDir(int dir)
    {
        if (Edges == null)
        {
            return false;
        }

        for (int i = 0; i < Edges.Length; i++)
        {
            Edge current = Edges[i];
            if (current != null && (int)current.Direction == dir)
            {
                return true;
            }
        }

        return false;
    }

    public MapPiece GetInstance()
    {
        PackedScene scene = GetScene();
        if (scene == null)
        {
            GD.PushError($"[MapPieceData] Scene could not be loaded from {GetScenePath()}.");
            return null;
        }

        Node instance = scene.Instantiate<Node>();
        if (instance == null)
        {
            GD.PushError("[MapPieceData] Could not instantiate scene.");
            return null;
        }

        MapPiece mapPiece = instance as MapPiece;
        if (mapPiece == null)
        {
            GD.PushError("[MapPieceData] Instanced node is not a MapPiece.");
            instance.QueueFree();
            return null;
        }

        var newEdges = new System.Collections.Generic.List<Edge>();
        if (Edges != null)
        {
            for (int i = 0; i < Edges.Length; i++)
            {
                Edge e = Edges[i];
                if (e != null)
                {
                    newEdges.Add(new Edge(e.Direction, e.Position));
                }
            }
        }

        mapPiece.edges = newEdges.ToArray();

        return mapPiece;
    }

    private PackedScene GetScene()
    {
        if (_cachedScene != null)
        {
            return _cachedScene;
        }

        string scenePath = GetScenePath();
        if (string.IsNullOrWhiteSpace(scenePath))
        {
            GD.PushError("[MapPieceData] Scene path is empty.");
            return null;
        }

        _cachedScene = ResourceLoader.Load<PackedScene>(scenePath);
        if (_cachedScene == null)
        {
            GD.PushError($"[MapPieceData] Resource is not a PackedScene or does not exist: {scenePath}");
        }

        return _cachedScene;
    }

    private string GetScenePath()
    {
        string sceneId = GetSceneId();
        if (string.IsNullOrWhiteSpace(sceneId))
        {
            return string.Empty;
        }

        string directory = GetSceneDirectory();
        return $"{directory}{sceneId}{SceneExtension}";
    }

    private string GetSceneId()
    {
        string sceneId = SceneId?.Trim() ?? string.Empty;
        if (sceneId.EndsWith(SceneExtension, StringComparison.OrdinalIgnoreCase))
        {
            sceneId = sceneId[..^SceneExtension.Length];
        }

        if (!string.IsNullOrWhiteSpace(sceneId))
        {
            return sceneId;
        }

        return GetFileBaseName(ResourcePath);
    }

    private string GetSceneDirectory()
    {
        return IsInitialPiece ? InitialSceneDirectory : DefaultSceneDirectory;
    }

    private static string GetFileBaseName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        int slashIndex = path.LastIndexOf('/');
        string fileName = slashIndex >= 0 ? path[(slashIndex + 1)..] : path;
        if (fileName.EndsWith(SceneExtension, StringComparison.OrdinalIgnoreCase))
        {
            return fileName[..^SceneExtension.Length];
        }

        int dotIndex = fileName.LastIndexOf('.');
        return dotIndex >= 0 ? fileName[..dotIndex] : fileName;
    }

}
