using Godot;

[GlobalClass]
public partial class LevelTileMap : TileMapLayer
{
    private const string BUILDEABLE = "buildeable";
    private const string BLOCKED = "blocked";
    private const int ATLAS_ID = 0;

    private static readonly Vector2I UNLOCK_TILE_POS = new(6, 0);
    private static readonly Vector2I NORMAL_TILE_POS = new(2, 0);

    private readonly Godot.Collections.Dictionary<Vector2I, bool> _occupiedTiles = new();
    private readonly Godot.Collections.Dictionary<Vector2I, bool> _blockedTiles = new();
    private readonly Godot.Collections.Dictionary<Vector2I, bool> _buildeableTiles = new();

    private TowersManager _towersManager;

    public override void _Ready()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        _towersManager = runContext?.TowersManager;
        if (_towersManager != null)
        {
            _towersManager.TowerRemoved += OnTowerRemoved;
        }

        FillData();
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.TowerRemoved -= OnTowerRemoved;
        }
    }

    public Vector2I get_mouse_tile_pos()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        return LocalToMap(ToLocal(mousePos));
    }

    public bool is_mouse_on_block_tile()
    {
        Vector2I mapCoords = get_mouse_tile_pos();
        TileData tileData = GetCellTileData(mapCoords);
        if (tileData == null)
        {
            return false;
        }

        return tileData.GetCustomData(BLOCKED).AsBool();
    }

    public bool is_mouse_on_buildeable_tile()
    {
        Vector2I mapCoords = get_mouse_tile_pos();
        if (!_buildeableTiles.ContainsKey(mapCoords))
        {
            return false;
        }

        return !_occupiedTiles.ContainsKey(mapCoords) && !_blockedTiles.ContainsKey(mapCoords);
    }

    public void destroy_random_buildeable_tile()
    {
        var buildeableTilesArray = new Godot.Collections.Array<Vector2I>();
        foreach (Vector2I tilePos in _buildeableTiles.Keys)
        {
            if (!_blockedTiles.ContainsKey(tilePos))
            {
                buildeableTilesArray.Add(tilePos);
            }
        }

        if (buildeableTilesArray.Count == 0)
        {
            return;
        }

        int randIndex = (int)(GD.Randi() % (uint)buildeableTilesArray.Count);
        Vector2I tileToBlock = buildeableTilesArray[randIndex];
        _occupiedTiles[tileToBlock] = true;
        SetCell(tileToBlock, ATLAS_ID, NORMAL_TILE_POS);
        _buildeableTiles.Remove(tileToBlock);
    }

    public Vector2 get_current_tile_pos()
    {
        Vector2 centerPosLocal = MapToLocal(get_mouse_tile_pos());
        centerPosLocal.Y -= 16;
        return ToGlobal(centerPosLocal);
    }

    public void set_tile_occupied(Vector2I map_coords)
    {
        _occupiedTiles[map_coords] = true;
        _buildeableTiles.Remove(map_coords);
    }

    public void set_tile_free(Vector2I map_coords)
    {
        if (_occupiedTiles.ContainsKey(map_coords))
        {
            _occupiedTiles.Remove(map_coords);
            _buildeableTiles[map_coords] = true;
        }
    }

    public void unblock_tile(Vector2I map_coords)
    {
        if (_blockedTiles.Count == 0)
        {
            return;
        }

        _blockedTiles.Remove(map_coords);
        SetCell(map_coords, ATLAS_ID, UNLOCK_TILE_POS);
        _buildeableTiles[map_coords] = true;
    }

    private void FillData()
    {
        Godot.Collections.Array<Vector2I> usedCells = GetUsedCells();
        for (int i = 0; i < usedCells.Count; i++)
        {
            Vector2I mapCoords = usedCells[i];
            TileData tileData = GetCellTileData(mapCoords);
            if (tileData == null)
            {
                continue;
            }

            if (tileData.GetCustomData(BLOCKED).AsBool())
            {
                _blockedTiles[mapCoords] = true;
            }
            else if (tileData.GetCustomData(BUILDEABLE).AsBool())
            {
                _buildeableTiles[mapCoords] = true;
            }
        }
    }

    private void OnTowerRemoved(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        Vector2I tile = tower.TilePos;
        set_tile_free(tile);
    }
}
