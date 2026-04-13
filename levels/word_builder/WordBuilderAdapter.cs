using System.Collections.Generic;
using Godot;

public interface IWordBuilderAdapter
{
    long GetObjectKey(object value);

    bool IsPieceValid(object piece);
    IList<object> GetPieceEdges(object piece);
    bool RemoveEdgeFromPiece(object piece, object edge);
    Vector2I GetPieceLogicalPos(object piece);

    int GetEdgeDir(object edge);
    int GetEdgePos(object edge);
    object GetOppositeEdge(object edge);
    bool EdgesMatch(object leftEdge, object rightEdge);

    bool PieceDataHasConnectingEdge(object pieceData, object edge);
    bool PieceDataHasEdgeDir(object pieceData, int dir);

    Vector2 GetPieceGlobalPosition(object piece);
    IList<Vector2> GetRouteWaypoints(object piece, int entryDir, int exitDir);
    IList<Vector2> GetFinalRouteWaypoints(object piece, int entryDir);
    int GetOppositeDir(int dir);

    bool PieceDataIsFork(object pieceData);
}
