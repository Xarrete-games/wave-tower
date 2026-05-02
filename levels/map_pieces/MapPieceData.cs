using System;
using Godot;

[GlobalClass]
public partial class MapPieceData : Resource
{
    [Export]
    public Edge[] Edges { get; set; } = Array.Empty<Edge>();

    [Export]
    public PackedScene Scene { get; set; }

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
        if (Scene == null)
        {
            GD.PushError("[MapPieceData] Scene is null in GetInstance().");
            return null;
        }

        Node instance = Scene.Instantiate<Node>();
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

}

