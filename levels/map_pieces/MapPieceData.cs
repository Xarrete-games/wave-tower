using Godot;

[GlobalClass]
public partial class MapPieceData : Resource
{
    [Export]
    public Godot.Collections.Array<Edge> edges { get; set; } = new();

    [Export]
    public PackedScene scene { get; set; }

    public bool is_fork
    {
        get => edges != null && edges.Count > 2;
    }

    public bool has_edge(Edge edge)
    {
        if (edge == null || edges == null)
        {
            return false;
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Edge current = edges[i];
            if (current != null && current.matches(edge))
            {
                return true;
            }
        }

        return false;
    }

    public bool has_connecting_edge(Edge edge)
    {
        if (edge == null || edges == null)
        {
            return false;
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Edge current = edges[i];
            if (current != null && current.can_connect_with(edge))
            {
                return true;
            }
        }

        return false;
    }

    public Edge get_connecting_edge(Edge edge)
    {
        if (edge == null || edges == null)
        {
            return null;
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Edge current = edges[i];
            if (current != null && current.can_connect_with(edge))
            {
                return current;
            }
        }

        return null;
    }

    public bool has_edge_dir(int dir)
    {
        if (edges == null)
        {
            return false;
        }

        for (int i = 0; i < edges.Count; i++)
        {
            Edge current = edges[i];
            if (current != null && (int)current.dir == dir)
            {
                return true;
            }
        }

        return false;
    }

    public Variant get_instance()
    {
        if (scene == null)
        {
            GD.PushError("[MapPieceData] scene is null in get_instance().");
            return default;
        }

        Node instance = scene.Instantiate<Node>();
        if (instance == null)
        {
            GD.PushError("[MapPieceData] Could not instantiate scene.");
            return default;
        }

        var newEdges = new Godot.Collections.Array<Edge>();
        if (edges != null)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                Edge e = edges[i];
                if (e != null)
                {
                    newEdges.Add(new Edge(e.dir, e.pos));
                }
            }
        }

        if (instance is MapPiece mapPiece)
        {
            mapPiece.edges = newEdges;
        }
        else
        {
            GD.PushWarning("[MapPieceData] Instanced node is not a MapPiece; edges were not assigned.");
        }

        return instance;
    }
}
