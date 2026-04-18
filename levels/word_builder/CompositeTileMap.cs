using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CompositeTileMap : Node
{
    private const string BUILDEABLE = "buildeable";
    private const string BLOCKED = "blocked";
    private const int ATLAS_ID = 0;
    private static readonly Vector2I UNLOCK_TILE_POS = new(6, 0);
    private static readonly Vector2I NORMAL_TILE_POS = new(2, 0);
    private const int MAX_BUILDEABLE_PER_PIECE = 5;

    private sealed class TileKey
    {
        public MapPiece Piece { get; }
        public Vector2I Coords { get; }

        public TileKey(MapPiece piece, Vector2I coords)
        {
            Piece = piece;
            Coords = coords;
        }
    }

    private readonly List<MapPiece> _pieces = new();
    private readonly Dictionary<string, bool> _occupiedTiles = new();
    private readonly Dictionary<string, bool> _blockedTiles = new();
    private readonly Dictionary<string, bool> _buildeableTiles = new();
    private readonly Dictionary<string, TileKey> _keyToTile = new();

    private TowersManager _towersManager;

    public override void _Ready()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.CompositeTileMap = this;

        _towersManager = runContext.towers_manager;
        if (_towersManager != null)
        {
            _towersManager.tower_removed += OnTowerRemoved;
        }
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.tower_removed -= OnTowerRemoved;
        }
    }

    public void register_piece(MapPiece piece)
    {
        if (piece == null || _pieces.Contains(piece))
        {
            return;
        }

        _pieces.Add(piece);
        piece.limit_buildeable_tiles(MAX_BUILDEABLE_PER_PIECE);
        ScanPiece(piece);
    }

    public void unregister_piece(MapPiece piece)
    {
        if (piece == null || !_pieces.Contains(piece))
        {
            return;
        }

        _pieces.Remove(piece);
        string prefix = $"{piece.GetInstanceId()}:";

        foreach (string key in new List<string>(_buildeableTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                _buildeableTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(_blockedTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                _blockedTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(_occupiedTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                _occupiedTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }
    }

    public Vector2I get_mouse_tile_pos()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return new Vector2I(-9999, -9999);
        }

        return info.Coords;
    }

    public bool is_mouse_on_buildeable_tile()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        if (!_buildeableTiles.ContainsKey(key))
        {
            return false;
        }

        return !_occupiedTiles.ContainsKey(key) && !_blockedTiles.ContainsKey(key);
    }

    public bool is_mouse_on_block_tile()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        return _blockedTiles.ContainsKey(key);
    }

    public Vector2 get_current_tile_pos()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return GetViewport().GetMousePosition();
        }

        TileMapLayer tileMap = GetTileMap(info.Piece);
        if (tileMap == null)
        {
            return GetViewport().GetMousePosition();
        }

        Vector2 centerLocal = tileMap.MapToLocal(info.Coords);
        centerLocal.Y -= 16;
        return tileMap.ToGlobal(centerLocal);
    }

    public string set_tile_occupied_at_mouse()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return string.Empty;
        }

        string key = MakeKey(info.Piece, info.Coords);
        _occupiedTiles[key] = true;
        _buildeableTiles.Remove(key);
        return key;
    }

    public void set_tile_occupied(string key)
    {
        _occupiedTiles[key] = true;
        _buildeableTiles.Remove(key);
    }

    public void set_tile_free(string key)
    {
        if (!_occupiedTiles.ContainsKey(key))
        {
            return;
        }

        _occupiedTiles.Remove(key);
        _buildeableTiles[key] = true;
    }

    public void unblock_tile(string key)
    {
        if (!_blockedTiles.ContainsKey(key))
        {
            return;
        }

        _blockedTiles.Remove(key);

        if (_keyToTile.TryGetValue(key, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = GetTileMap(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, UNLOCK_TILE_POS);
        }

        _buildeableTiles[key] = true;
    }

    public bool unblock_tile_at_mouse()
    {
        TileKey info = get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        if (!_blockedTiles.ContainsKey(key))
        {
            return false;
        }

        unblock_tile(key);
        return true;
    }

    public void destroy_random_buildeable_tile()
    {
        List<string> candidates = new();
        foreach (string key in _buildeableTiles.Keys)
        {
            if (!_blockedTiles.ContainsKey(key))
            {
                candidates.Add(key);
            }
        }

        if (candidates.Count == 0)
        {
            return;
        }

        int randomIndex = (int)(GD.Randi() % (uint)candidates.Count);
        string selectedKey = candidates[randomIndex];

        _occupiedTiles[selectedKey] = true;
        _buildeableTiles.Remove(selectedKey);

        if (_keyToTile.TryGetValue(selectedKey, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = GetTileMap(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, NORMAL_TILE_POS);
        }
    }

    private TileKey get_mouse_tile_info()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Transform2D canvasTransform = GetViewport().GetCanvasTransform();
        Vector2 globalMouse = canvasTransform.AffineInverse() * mousePos;

        for (int index = 0; index < _pieces.Count; index++)
        {
            MapPiece piece = _pieces[index];
            TileMapLayer tileMap = GetTileMap(piece);
            if (tileMap == null)
            {
                continue;
            }

            Vector2 localPos = tileMap.ToLocal(globalMouse);
            Vector2I mapCoords = tileMap.LocalToMap(localPos);
            if (tileMap.GetCellSourceId(mapCoords) != -1)
            {
                return new TileKey(piece, mapCoords);
            }
        }

        return null;
    }

    private void ScanPiece(MapPiece piece)
    {
        TileMapLayer tileMap = GetTileMap(piece);
        if (tileMap == null)
        {
            return;
        }

        Godot.Collections.Array<Vector2I> usedCells = tileMap.GetUsedCells();
        for (int index = 0; index < usedCells.Count; index++)
        {
            Vector2I mapCoords = usedCells[index];
            TileData tileData = tileMap.GetCellTileData(mapCoords);
            if (tileData == null)
            {
                continue;
            }

            string key = MakeKey(piece, mapCoords);
            _keyToTile[key] = new TileKey(piece, mapCoords);

            if ((bool)tileData.GetCustomData(BLOCKED))
            {
                _blockedTiles[key] = true;
            }
            else if ((bool)tileData.GetCustomData(BUILDEABLE))
            {
                _buildeableTiles[key] = true;
            }
        }
    }

    private string MakeKey(MapPiece piece, Vector2I coords)
    {
        return $"{piece.GetInstanceId()}:{coords.X},{coords.Y}";
    }

    private TileMapLayer GetTileMap(MapPiece piece)
    {
        return piece?.GetNodeOrNull<TileMapLayer>("MapPieceTileMap");
    }

    private void OnTowerRemoved(Tower tower)
    {
        string key = tower?.CompositeTileKey;
        if (!string.IsNullOrEmpty(key))
        {
            set_tile_free(key);
        }
    }
}
