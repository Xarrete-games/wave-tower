using Godot;

public sealed class SpawnEntry
{
    public string Key { get; set; } = string.Empty;

    public Vector2I Tile { get; set; } = Vector2I.Zero;

    public Vector2 Position { get; set; } = Vector2.Zero;

    public Edge Edge { get; set; }

    public MapPiece Piece { get; set; }

    public int Dir => Edge != null ? (int)Edge.Direction : -1;
}
