using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class CompositeTileMap : Node
{
    private const string BuildableCustomDataKey = "buildeable";
    private const string BLOCKED = "blocked";
    private const int ATLAS_ID = 0;
    private static readonly Vector2I UNLOCK_TILE_POS = new(6, 0);
    private static readonly Vector2I NORMAL_TILE_POS = new(2, 0);
    private const int MaxBuildablePerPiece = 5;

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
    private readonly HashSet<string> _occupiedTiles = new();
    private readonly HashSet<string> _blockedTiles = new();
    private readonly HashSet<string> _buildableTiles = new();
    private readonly Dictionary<string, TileKey> _keyToTile = new();

    private TowersManager _towersManager;

    public override void _Ready()
    {
        RunContext runContext = RunContext.Instance;
        runContext.SetCompositeTileMap(this);

        _towersManager = runContext.TowersManager;
        if (_towersManager != null)
        {
            _towersManager.TowerRemoved += OnTowerRemoved;
        }
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.TowerRemoved -= OnTowerRemoved;
        }
    }

    public void RegisterPiece(MapPiece piece)
    {
        if (piece == null || _pieces.Contains(piece))
        {
            return;
        }

        _pieces.Add(piece);
        piece.LimitBuildableTiles(MaxBuildablePerPiece);
        ScanPiece(piece);
    }

    public void UnregisterPiece(MapPiece piece)
    {
        if (piece == null || !_pieces.Contains(piece))
        {
            return;
        }

        _pieces.Remove(piece);
        string prefix = $"{piece.GetInstanceId()}:";

        foreach (string key in new List<string>(_buildableTiles))
        {
            if (key.StartsWith(prefix))
            {
                _buildableTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(_blockedTiles))
        {
            if (key.StartsWith(prefix))
            {
                _blockedTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(_occupiedTiles))
        {
            if (key.StartsWith(prefix))
            {
                _occupiedTiles.Remove(key);
                _keyToTile.Remove(key);
            }
        }
    }

    public Vector2I GetMouseTilePos()
    {
        TileKey info = GetMouseTileInfo();
        if (info == null)
        {
            return new Vector2I(-9999, -9999);
        }

        return info.Coords;
    }

    public bool IsMouseOnBuildableTile()
    {
        TileKey info = GetMouseTileInfo();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        if (!_buildableTiles.Contains(key))
        {
            return false;
        }

        return !_occupiedTiles.Contains(key) && !_blockedTiles.Contains(key);
    }

    public bool IsMouseOnBlockTile()
    {
        TileKey info = GetMouseTileInfo();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        return _blockedTiles.Contains(key);
    }

    public Vector2 GetCurrentTilePos()
    {
        TileKey info = GetMouseTileInfo();
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

    public string SetTileOccupiedAtMouse()
    {
        TileKey info = GetMouseTileInfo();
        if (info == null)
        {
            return string.Empty;
        }

        string key = MakeKey(info.Piece, info.Coords);
        _occupiedTiles.Add(key);
        _buildableTiles.Remove(key);
        return key;
    }

    public void SetTileOccupied(string key)
    {
        _occupiedTiles.Add(key);
        _buildableTiles.Remove(key);
    }

    public void SetTileFree(string key)
    {
        if (!_occupiedTiles.Contains(key))
        {
            return;
        }

        _occupiedTiles.Remove(key);
        _buildableTiles.Add(key);
    }

    public void UnblockTile(string key)
    {
        if (!_blockedTiles.Contains(key))
        {
            return;
        }

        _blockedTiles.Remove(key);

        if (_keyToTile.TryGetValue(key, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = GetTileMap(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, UNLOCK_TILE_POS);
        }

        _buildableTiles.Add(key);
    }

    public bool UnblockTileAtMouse()
    {
        TileKey info = GetMouseTileInfo();
        if (info == null)
        {
            return false;
        }

        string key = MakeKey(info.Piece, info.Coords);
        if (!_blockedTiles.Contains(key))
        {
            return false;
        }

        UnblockTile(key);
        return true;
    }

    public void DestroyRandomBuildableTile()
    {
        List<string> candidates = new();
        foreach (string key in _buildableTiles)
        {
            if (!_blockedTiles.Contains(key))
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

        _occupiedTiles.Add(selectedKey);
        _buildableTiles.Remove(selectedKey);

        if (_keyToTile.TryGetValue(selectedKey, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = GetTileMap(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, NORMAL_TILE_POS);
        }
    }

    private TileKey GetMouseTileInfo()
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

        var usedCells = tileMap.GetUsedCells();
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
                _blockedTiles.Add(key);
            }
            else if ((bool)tileData.GetCustomData(BuildableCustomDataKey))
            {
                _buildableTiles.Add(key);
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

    private void OnTowerRemoved(TowerNode tower)
    {
        string key = tower?.CompositeTileKey;
        if (!string.IsNullOrEmpty(key))
        {
            SetTileFree(key);
        }
    }
}
