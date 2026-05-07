using Godot;

[GlobalClass]
public partial class Edge : Resource
{
    public enum Dir
    {
        NE,
        SE,
        SW,
        NW,
    }

    public enum DirPos
    {
        TOP,
        MIDDLE,
        BOTTOM,
    }

    [Export]
    public Dir Direction { get; set; } = Dir.NE;

    [Export]
    public DirPos Position { get; set; } = DirPos.MIDDLE;

    public Edge()
    {
    }

    public Edge(Dir edgeDir, DirPos edgePos = DirPos.MIDDLE)
    {
        Direction = edgeDir;
        Position = edgePos;
    }

    public Edge GetOpposite()
    {
        return new Edge(GetOppositeDir(Direction), Position);
    }

    public bool Matches(Edge other)
    {
        return other != null && Direction == other.Direction && Position == other.Position;
    }

    public bool CanConnectWith(Edge other)
    {
        return other != null && Direction == GetOppositeDir(other.Direction) && Position == other.Position;
    }

    public static Dir GetOppositeDir(Dir d)
    {
        return d switch
        {
            Dir.NE => Dir.SW,
            Dir.SE => Dir.NW,
            Dir.SW => Dir.NE,
            Dir.NW => Dir.SE,
            _ => Dir.NE,
        };
    }

    public static Edge FromDir(Dir d)
    {
        return new Edge(d, DirPos.MIDDLE);
    }

    public override string ToString()
    {
        string[] dirStr = { "NE", "SE", "SW", "NW" };
        string[] posStr = { "TOP", "MIDDLE", "BOTTOM" };
        return $"{dirStr[(int)Direction]}_{posStr[(int)Position]}";
    }
}

