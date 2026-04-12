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
    public Dir dir { get; set; } = Dir.NE;

    [Export]
    public DirPos pos { get; set; } = DirPos.MIDDLE;

    public Edge()
    {
    }

    public Edge(Dir p_dir, DirPos p_pos = DirPos.MIDDLE)
    {
        this.dir = p_dir;
        this.pos = p_pos;
    }

    public Edge get_opposite()
    {
        return new Edge(get_opposite_dir(this.dir), this.pos);
    }

    public bool matches(Edge other)
    {
        return other != null && this.dir == other.dir && this.pos == other.pos;
    }

    public bool can_connect_with(Edge other)
    {
        return other != null && this.dir == get_opposite_dir(other.dir) && this.pos == other.pos;
    }

    public static Dir get_opposite_dir(Dir d)
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

    public static Edge from_dir(Dir d)
    {
        return new Edge(d, DirPos.MIDDLE);
    }

    public override string ToString()
    {
        string[] dirStr = { "NE", "SE", "SW", "NW" };
        string[] posStr = { "TOP", "MIDDLE", "BOTTOM" };
        return $"{dirStr[(int)this.dir]}_{posStr[(int)this.pos]}";
    }
}
