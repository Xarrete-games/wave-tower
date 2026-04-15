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
            this.Piece = piece;
            this.Coords = coords;
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
        runContext.composite_tile_map = this;

        this._towersManager = runContext.towers_manager;
        if (this._towersManager != null)
        {
            this._towersManager.tower_removed += this._on_tower_removed;
        }
    }

    public override void _ExitTree()
    {
        if (this._towersManager != null)
        {
            this._towersManager.tower_removed -= this._on_tower_removed;
        }
    }

    public void register_piece(MapPiece piece)
    {
        if (piece == null || this._pieces.Contains(piece))
        {
            return;
        }

        this._pieces.Add(piece);
        piece.limit_buildeable_tiles(MAX_BUILDEABLE_PER_PIECE);
        this._scan_piece(piece);
    }

    public void unregister_piece(MapPiece piece)
    {
        if (piece == null || !this._pieces.Contains(piece))
        {
            return;
        }

        this._pieces.Remove(piece);
        string prefix = $"{piece.GetInstanceId()}:";

        foreach (string key in new List<string>(this._buildeableTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                this._buildeableTiles.Remove(key);
                this._keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(this._blockedTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                this._blockedTiles.Remove(key);
                this._keyToTile.Remove(key);
            }
        }

        foreach (string key in new List<string>(this._occupiedTiles.Keys))
        {
            if (key.StartsWith(prefix))
            {
                this._occupiedTiles.Remove(key);
                this._keyToTile.Remove(key);
            }
        }
    }

    public Vector2I get_mouse_tile_pos()
    {
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return new Vector2I(-9999, -9999);
        }

        return info.Coords;
    }

    public bool is_mouse_on_buildeable_tile()
    {
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = this._make_key(info.Piece, info.Coords);
        if (!this._buildeableTiles.ContainsKey(key))
        {
            return false;
        }

        return !this._occupiedTiles.ContainsKey(key) && !this._blockedTiles.ContainsKey(key);
    }

    public bool is_mouse_on_block_tile()
    {
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = this._make_key(info.Piece, info.Coords);
        return this._blockedTiles.ContainsKey(key);
    }

    public Vector2 get_current_tile_pos()
    {
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return GetViewport().GetMousePosition();
        }

        TileMapLayer tileMap = this._get_tile_map(info.Piece);
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
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return string.Empty;
        }

        string key = this._make_key(info.Piece, info.Coords);
        this._occupiedTiles[key] = true;
        this._buildeableTiles.Remove(key);
        return key;
    }

    public void set_tile_occupied(string key)
    {
        this._occupiedTiles[key] = true;
        this._buildeableTiles.Remove(key);
    }

    public void set_tile_free(string key)
    {
        if (!this._occupiedTiles.ContainsKey(key))
        {
            return;
        }

        this._occupiedTiles.Remove(key);
        this._buildeableTiles[key] = true;
    }

    public void unblock_tile(string key)
    {
        if (!this._blockedTiles.ContainsKey(key))
        {
            return;
        }

        this._blockedTiles.Remove(key);

        if (this._keyToTile.TryGetValue(key, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = this._get_tile_map(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, UNLOCK_TILE_POS);
        }

        this._buildeableTiles[key] = true;
    }

    public bool unblock_tile_at_mouse()
    {
        TileKey info = this.get_mouse_tile_info();
        if (info == null)
        {
            return false;
        }

        string key = this._make_key(info.Piece, info.Coords);
        if (!this._blockedTiles.ContainsKey(key))
        {
            return false;
        }

        this.unblock_tile(key);
        return true;
    }

    public void destroy_random_buildeable_tile()
    {
        List<string> candidates = new();
        foreach (string key in this._buildeableTiles.Keys)
        {
            if (!this._blockedTiles.ContainsKey(key))
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

        this._occupiedTiles[selectedKey] = true;
        this._buildeableTiles.Remove(selectedKey);

        if (this._keyToTile.TryGetValue(selectedKey, out TileKey tile) && tile != null)
        {
            TileMapLayer tileMap = this._get_tile_map(tile.Piece);
            tileMap?.SetCell(tile.Coords, ATLAS_ID, NORMAL_TILE_POS);
        }
    }

    private TileKey get_mouse_tile_info()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Transform2D canvasTransform = GetViewport().GetCanvasTransform();
        Vector2 globalMouse = canvasTransform.AffineInverse() * mousePos;

        for (int index = 0; index < this._pieces.Count; index++)
        {
            MapPiece piece = this._pieces[index];
            TileMapLayer tileMap = this._get_tile_map(piece);
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

    private void _scan_piece(MapPiece piece)
    {
        TileMapLayer tileMap = this._get_tile_map(piece);
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

            string key = this._make_key(piece, mapCoords);
            this._keyToTile[key] = new TileKey(piece, mapCoords);

            if ((bool)tileData.GetCustomData(BLOCKED))
            {
                this._blockedTiles[key] = true;
            }
            else if ((bool)tileData.GetCustomData(BUILDEABLE))
            {
                this._buildeableTiles[key] = true;
            }
        }
    }

    private string _make_key(MapPiece piece, Vector2I coords)
    {
        return $"{piece.GetInstanceId()}:{coords.X},{coords.Y}";
    }

    private TileMapLayer _get_tile_map(MapPiece piece)
    {
        return piece?.GetNodeOrNull<TileMapLayer>("MapPieceTileMap");
    }

    private void _on_tower_removed(Tower tower)
    {
        string key = tower?.Get("composite_tile_key").AsString();
        if (!string.IsNullOrEmpty(key))
        {
            this.set_tile_free(key);
        }
    }
}
