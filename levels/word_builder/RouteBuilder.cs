using Godot;

[GlobalClass]
public partial class RouteBuilder : RefCounted
{
    private const int EdgeDirNe = 0;

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
                entryDir = (int)Edge.get_opposite_dir((Edge.Dir)entryDir);
            }

            if (hasExit)
            {
                GodotObject nextPiece = route[index + 1].AsGodotObject();
                exitDir = this._connectionGraph.find_connection_dir(piece, nextPiece);
            }

            if (hasExit)
            {
                Godot.Collections.Array<Vector2> intermediate = this._piece_get_route_waypoints(piece, entryDir, exitDir);
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
                Godot.Collections.Array<Vector2> toEnd = this._piece_get_final_route_waypoints(piece, entryDir);
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

    private static bool _has_method(GodotObject target, string methodName)
    {
        return target != null && GodotObject.IsInstanceValid(target) && target.HasMethod(methodName);
    }

    private Godot.Collections.Array<Vector2> _piece_get_route_waypoints(GodotObject piece, int entryDir, int exitDir)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_route_waypoints(entryDir, exitDir);
        }

        if (_has_method(piece, "get_route_waypoints"))
        {
            return piece.Call("get_route_waypoints", entryDir, exitDir).AsGodotArray<Vector2>();
        }

        if (_has_method(piece, "GetRouteWaypoints"))
        {
            return piece.Call("GetRouteWaypoints", entryDir, exitDir).AsGodotArray<Vector2>();
        }

        GD.PushError("[RouteBuilder] Piece has no route waypoint method.");
        return new Godot.Collections.Array<Vector2>();
    }

    private Godot.Collections.Array<Vector2> _piece_get_final_route_waypoints(GodotObject piece, int entryDir)
    {
        if (piece is MapPiece mapPiece)
        {
            return mapPiece.get_final_route_waypoints(entryDir);
        }

        if (_has_method(piece, "get_final_route_waypoints"))
        {
            return piece.Call("get_final_route_waypoints", entryDir).AsGodotArray<Vector2>();
        }

        if (_has_method(piece, "GetFinalRouteWaypoints"))
        {
            return piece.Call("GetFinalRouteWaypoints", entryDir).AsGodotArray<Vector2>();
        }

        GD.PushError("[RouteBuilder] Piece has no final route waypoint method.");
        return new Godot.Collections.Array<Vector2>();
    }
}
