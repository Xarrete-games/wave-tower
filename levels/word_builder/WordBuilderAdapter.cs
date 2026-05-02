using System.Collections.Generic;
using Godot;

public interface IWordBuilderAdapter
{
    long GetObjectKey(MapPiece value);

    bool IsPieceValid(MapPiece piece);
    IList<Edge> GetPieceEdges(MapPiece piece);
    bool RemoveEdgeFromPiece(MapPiece piece, Edge edge);
    Vector2I GetPieceLogicalPos(MapPiece piece);

    int GetEdgeDir(Edge edge);
    int GetEdgePos(Edge edge);
    Edge GetOppositeEdge(Edge edge);
    bool EdgesMatch(Edge leftEdge, Edge rightEdge);

    bool PieceDataHasConnectingEdge(MapPieceData pieceData, Edge edge);
    bool PieceDataHasEdgeDir(MapPieceData pieceData, int dir);

    Vector2 GetPieceGlobalPosition(MapPiece piece);
    IList<Vector2> GetRouteWaypoints(MapPiece piece, int entryDir, int exitDir);
    IList<Vector2> GetFinalRouteWaypoints(MapPiece piece, int entryDir);
    int GetOppositeDir(int dir);

    bool PieceDataIsFork(MapPieceData pieceData);
}
