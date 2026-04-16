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
        this._towersManager = runContext?.towers_manager;
        if (this._towersManager != null)
        {
            this._towersManager.tower_removed += this._on_tower_removed;
        }

        this._fill_data();
    }

    public override void _ExitTree()
    {
        if (this._towersManager != null)
        {
            this._towersManager.tower_removed -= this._on_tower_removed;
        }
    }

    public Vector2I get_mouse_tile_pos()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        return LocalToMap(ToLocal(mousePos));
    }

    public bool is_mouse_on_block_tile()
    {
        Vector2I mapCoords = this.get_mouse_tile_pos();
        TileData tileData = GetCellTileData(mapCoords);
        if (tileData == null)
        {
            return false;
        }

        return tileData.GetCustomData(BLOCKED).AsBool();
    }

    public bool is_mouse_on_buildeable_tile()
    {
        Vector2I mapCoords = this.get_mouse_tile_pos();
        if (!this._buildeableTiles.ContainsKey(mapCoords))
        {
            return false;
        }

        return !this._occupiedTiles.ContainsKey(mapCoords) && !this._blockedTiles.ContainsKey(mapCoords);
    }

    public void destroy_random_buildeable_tile()
    {
        var buildeableTilesArray = new Godot.Collections.Array<Vector2I>();
        foreach (Vector2I tilePos in this._buildeableTiles.Keys)
        {
            if (!this._blockedTiles.ContainsKey(tilePos))
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
        this._occupiedTiles[tileToBlock] = true;
        SetCell(tileToBlock, ATLAS_ID, NORMAL_TILE_POS);
        this._buildeableTiles.Remove(tileToBlock);
    }

    public Vector2 get_current_tile_pos()
    {
        Vector2 centerPosLocal = MapToLocal(this.get_mouse_tile_pos());
        centerPosLocal.Y -= 16;
        return ToGlobal(centerPosLocal);
    }

    public void set_tile_occupied(Vector2I map_coords)
    {
        this._occupiedTiles[map_coords] = true;
        this._buildeableTiles.Remove(map_coords);
    }

    public void set_tile_free(Vector2I map_coords)
    {
        if (this._occupiedTiles.ContainsKey(map_coords))
        {
            this._occupiedTiles.Remove(map_coords);
            this._buildeableTiles[map_coords] = true;
        }
    }

    public void unblock_tile(Vector2I map_coords)
    {
        if (this._blockedTiles.Count == 0)
        {
            return;
        }

        this._blockedTiles.Remove(map_coords);
        SetCell(map_coords, ATLAS_ID, UNLOCK_TILE_POS);
        this._buildeableTiles[map_coords] = true;
    }

    private void _fill_data()
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
                this._blockedTiles[mapCoords] = true;
            }
            else if (tileData.GetCustomData(BUILDEABLE).AsBool())
            {
                this._buildeableTiles[mapCoords] = true;
            }
        }
    }

    private void _on_tower_removed(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        Vector2I tile = tower.tile_pos;
        this.set_tile_free(tile);
    }
}
