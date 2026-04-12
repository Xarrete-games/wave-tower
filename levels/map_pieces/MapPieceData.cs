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
        get => this.edges != null && this.edges.Count > 2;
    }

    public bool has_edge(Edge edge)
    {
        if (edge == null || this.edges == null)
        {
            return false;
        }

        for (int i = 0; i < this.edges.Count; i++)
        {
            Edge current = this.edges[i];
            if (current != null && current.matches(edge))
            {
                return true;
            }
        }

        return false;
    }

    public bool has_connecting_edge(Edge edge)
    {
        if (edge == null || this.edges == null)
        {
            return false;
        }

        for (int i = 0; i < this.edges.Count; i++)
        {
            Edge current = this.edges[i];
            if (current != null && current.can_connect_with(edge))
            {
                return true;
            }
        }

        return false;
    }

    public Edge get_connecting_edge(Edge edge)
    {
        if (edge == null || this.edges == null)
        {
            return null;
        }

        for (int i = 0; i < this.edges.Count; i++)
        {
            Edge current = this.edges[i];
            if (current != null && current.can_connect_with(edge))
            {
                return current;
            }
        }

        return null;
    }

    public bool has_edge_dir(int dir)
    {
        if (this.edges == null)
        {
            return false;
        }

        for (int i = 0; i < this.edges.Count; i++)
        {
            Edge current = this.edges[i];
            if (current != null && (int)current.dir == dir)
            {
                return true;
            }
        }

        return false;
    }

    public Variant get_instance()
    {
        if (this.scene == null)
        {
            GD.PushError("[MapPieceData] scene is null in get_instance().");
            return default;
        }

        Node instance = this.scene.Instantiate<Node>();
        if (instance == null)
        {
            GD.PushError("[MapPieceData] Could not instantiate scene.");
            return default;
        }

        var newEdges = new Godot.Collections.Array<Edge>();
        if (this.edges != null)
        {
            for (int i = 0; i < this.edges.Count; i++)
            {
                Edge e = this.edges[i];
                if (e != null)
                {
                    newEdges.Add(new Edge(e.dir, e.pos));
                }
            }
        }

        instance.Set("edges", newEdges);
        return instance;
    }
}
