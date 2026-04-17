using System.Collections.Generic;
using Godot;

public class RouteBuilder
{
    private const int EdgeDirNe = 0;

    private readonly IWordBuilderAdapter _adapter;
    private PieceConnectionGraph _connectionGraph;
    private object _targetPiece;

    public RouteBuilder(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void setup(PieceConnectionGraph graph, object target)
    {
        _connectionGraph = graph;
        _targetPiece = target;
    }

    public List<object> build_route_to_target(Dictionary<string, object> spawnEntry)
    {
        var empty = new List<object>();
        if (!spawnEntry.TryGetValue("piece", out object startPiece))
        {
            GD.PushError("[RouteBuilder] spawn_entry has no 'piece'");
            return empty;
        }

        if (!_adapter.IsPieceValid(startPiece))
        {
            GD.PushError("[RouteBuilder] spawn_entry.piece is not valid");
            return empty;
        }

        if (ReferenceEquals(startPiece, _targetPiece))
        {
            empty.Add(_targetPiece);
            return empty;
        }

        return _connectionGraph.find_path(startPiece, _targetPiece);
    }

    public List<Vector2> build_waypoints_from_route(Dictionary<string, object> spawnEntry, List<object> route)
    {
        var waypoints = new List<Vector2>();
        if (route.Count == 0)
        {
            GD.PushWarning("[RouteBuilder] Empty route, cannot generate waypoints");
            return waypoints;
        }

        if (spawnEntry.TryGetValue("pos", out object spawnPosObj) && spawnPosObj is Vector2 spawnPos)
        {
            waypoints.Add(spawnPos);
        }

        for (int index = 0; index < route.Count; index++)
        {
            object piece = route[index];
            if (!_adapter.IsPieceValid(piece))
            {
                continue;
            }

            int entryDir = EdgeDirNe;
            int exitDir = EdgeDirNe;
            bool hasExit = index < route.Count - 1;

            if (index == 0)
            {
                if (spawnEntry.TryGetValue("edge", out object edgeObj))
                {
                    entryDir = _adapter.GetEdgeDir(edgeObj);
                }
            }
            else
            {
                object prevPiece = route[index - 1];
                entryDir = _connectionGraph.find_connection_dir(prevPiece, piece);
                entryDir = _adapter.GetOppositeDir(entryDir);
            }

            if (hasExit)
            {
                object nextPiece = route[index + 1];
                exitDir = _connectionGraph.find_connection_dir(piece, nextPiece);
            }

            if (hasExit)
            {
                IList<Vector2> intermediate = _adapter.GetRouteWaypoints(piece, entryDir, exitDir);
                if (intermediate.Count > 0)
                {
                    Vector2 globalPosition = _adapter.GetPieceGlobalPosition(piece);
                    for (int i = 0; i < intermediate.Count; i++)
                    {
                        waypoints.Add(globalPosition + intermediate[i]);
                    }
                }
                else
                {
                    waypoints.Add(_adapter.GetPieceGlobalPosition(piece));
                }
            }
            else
            {
                IList<Vector2> toEnd = _adapter.GetFinalRouteWaypoints(piece, entryDir);
                if (toEnd.Count > 0)
                {
                    Vector2 globalPosition = _adapter.GetPieceGlobalPosition(piece);
                    for (int i = 0; i < toEnd.Count; i++)
                    {
                        waypoints.Add(globalPosition + toEnd[i]);
                    }
                }
                else
                {
                    waypoints.Add(_adapter.GetPieceGlobalPosition(piece));
                }
            }
        }

        return waypoints;
    }

    public List<Vector2> get_waypoints_for_spawn(Dictionary<string, object> spawnEntry)
    {
        List<object> route = build_route_to_target(spawnEntry);
        return build_waypoints_from_route(spawnEntry, route);
    }
}
