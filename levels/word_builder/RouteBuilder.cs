using Godot;

[GlobalClass]
public partial class RouteBuilder : RefCounted
{
    private const int EdgeDirNe = 0;
    private readonly Script _edgeScript = GD.Load<Script>("res://levels/map_pieces/edge.gd");

    private PieceConnectionGraph _connectionGraph;
    private GodotObject _targetPiece;

    public void setup(PieceConnectionGraph graph, Variant target)
    {
        this._connectionGraph = graph;
        this._targetPiece = target.AsGodotObject();
    }

    public Godot.Collections.Array<Variant> build_route_to_target(Godot.Collections.Dictionary spawn_entry)
    {
        var empty = new Godot.Collections.Array<Variant>();
        if (!spawn_entry.ContainsKey("piece"))
        {
            GD.PushError("[RouteBuilder] spawn_entry has no 'piece'");
            return empty;
        }

        GodotObject startPiece = spawn_entry["piece"].AsGodotObject();
        if (startPiece == null || !GodotObject.IsInstanceValid(startPiece))
        {
            GD.PushError("[RouteBuilder] spawn_entry.piece is not valid");
            return empty;
        }

        if (ReferenceEquals(startPiece, this._targetPiece))
        {
            empty.Add(this._targetPiece);
            return empty;
        }

        return this._connectionGraph.find_path(startPiece, this._targetPiece);
    }

    public Godot.Collections.Array<Vector2> build_waypoints_from_route(Godot.Collections.Dictionary spawn_entry, Godot.Collections.Array<Variant> route)
    {
        var waypoints = new Godot.Collections.Array<Vector2>();
        if (route.Count == 0)
        {
            GD.PushWarning("[RouteBuilder] Empty route, cannot generate waypoints");
            return waypoints;
        }

        if (spawn_entry.ContainsKey("pos"))
        {
            waypoints.Add(spawn_entry["pos"].AsVector2());
        }

        for (int index = 0; index < route.Count; index++)
        {
            GodotObject piece = route[index].AsGodotObject();
            if (piece == null)
            {
                continue;
            }

            int entryDir = EdgeDirNe;
            int exitDir = EdgeDirNe;
            bool hasExit = index < route.Count - 1;

            if (index == 0)
            {
                if (spawn_entry.ContainsKey("edge"))
                {
                    entryDir = spawn_entry["edge"].AsGodotObject()?.Get("dir").AsInt32() ?? EdgeDirNe;
                }
            }
            else
            {
                GodotObject prevPiece = route[index - 1].AsGodotObject();
                entryDir = this._connectionGraph.find_connection_dir(prevPiece, piece);
                entryDir = this._edgeScript.Call("get_opposite_dir", entryDir).AsInt32();
            }

            if (hasExit)
            {
                GodotObject nextPiece = route[index + 1].AsGodotObject();
                exitDir = this._connectionGraph.find_connection_dir(piece, nextPiece);
            }

            if (hasExit)
            {
                Godot.Collections.Array<Vector2> intermediate = piece.Call("get_route_waypoints", entryDir, exitDir).AsGodotArray<Vector2>();
                if (intermediate.Count > 0)
                {
                    for (int i = 0; i < intermediate.Count; i++)
                    {
                        waypoints.Add(piece.Get("global_position").AsVector2() + intermediate[i]);
                    }
                }
                else
                {
                    waypoints.Add(piece.Get("global_position").AsVector2());
                }
            }
            else
            {
                Godot.Collections.Array<Vector2> toEnd = piece.Call("get_final_route_waypoints", entryDir).AsGodotArray<Vector2>();
                if (toEnd.Count > 0)
                {
                    for (int i = 0; i < toEnd.Count; i++)
                    {
                        waypoints.Add(piece.Get("global_position").AsVector2() + toEnd[i]);
                    }
                }
                else
                {
                    waypoints.Add(piece.Get("global_position").AsVector2());
                }
            }
        }

        return waypoints;
    }

    public Godot.Collections.Array<Vector2> get_waypoints_for_spawn(Godot.Collections.Dictionary spawn_entry)
    {
        Godot.Collections.Array<Variant> route = this.build_route_to_target(spawn_entry);
        return this.build_waypoints_from_route(spawn_entry, route);
    }
}
