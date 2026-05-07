using System.Collections.Generic;
using Godot;

public class RouteBuilder
{
    private const int EdgeDirNe = 0;

    private readonly IWordBuilderAdapter _adapter;
    private PieceConnectionGraph _connectionGraph;
    private MapPiece _targetPiece;

    public RouteBuilder(IWordBuilderAdapter adapter)
    {
        _adapter = adapter;
    }

    public void Setup(PieceConnectionGraph graph, MapPiece target)
    {
        _connectionGraph = graph;
        _targetPiece = target;
    }

    public List<MapPiece> BuildRouteToTarget(SpawnEntry spawnEntry)
    {
        var empty = new List<MapPiece>();
        MapPiece startPiece = spawnEntry?.Piece;
        if (startPiece == null)
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

        return _connectionGraph.FindPath(startPiece, _targetPiece);
    }

    public List<Vector2> BuildWaypointsFromRoute(SpawnEntry spawnEntry, List<MapPiece> route)
    {
        var waypoints = new List<Vector2>();
        if (route.Count == 0)
        {
            GD.PushWarning("[RouteBuilder] Empty route, cannot generate waypoints");
            return waypoints;
        }

        waypoints.Add(spawnEntry.Position);

        for (int index = 0; index < route.Count; index++)
        {
            MapPiece piece = route[index];
            if (!_adapter.IsPieceValid(piece))
            {
                continue;
            }

            int entryDir = EdgeDirNe;
            int exitDir = EdgeDirNe;
            bool hasExit = index < route.Count - 1;

            if (index == 0)
            {
                if (spawnEntry?.Edge != null)
                {
                    entryDir = _adapter.GetEdgeDir(spawnEntry.Edge);
                }
            }
            else
            {
                MapPiece prevPiece = route[index - 1];
                entryDir = _connectionGraph.FindConnectionDir(prevPiece, piece);
                entryDir = _adapter.GetOppositeDir(entryDir);
            }

            if (hasExit)
            {
                MapPiece nextPiece = route[index + 1];
                exitDir = _connectionGraph.FindConnectionDir(piece, nextPiece);
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

    public List<Vector2> GetWaypointsForSpawn(SpawnEntry spawnEntry)
    {
        List<MapPiece> route = BuildRouteToTarget(spawnEntry);
        return BuildWaypointsFromRoute(spawnEntry, route);
    }
}
