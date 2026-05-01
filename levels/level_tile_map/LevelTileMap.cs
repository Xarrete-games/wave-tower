using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class LevelTileMap : TileMapLayer
{
    private const string BuildableCustomDataKey = "buildeable";
    private const string BLOCKED = "blocked";
    private const int ATLAS_ID = 0;

    private static readonly Vector2I UNLOCK_TILE_POS = new(6, 0);
    private static readonly Vector2I NORMAL_TILE_POS = new(2, 0);

    private readonly HashSet<Vector2I> _occupiedTiles = new();
    private readonly HashSet<Vector2I> _blockedTiles = new();
    private readonly HashSet<Vector2I> _buildableTiles = new();

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

    public Vector2I GetMouseTilePos()
    {
        Vector2 mousePos = GetGlobalMousePosition();
        return LocalToMap(ToLocal(mousePos));
    }

    public bool IsMouseOnBlockTile()
    {
        Vector2I mapCoords = GetMouseTilePos();
        TileData tileData = GetCellTileData(mapCoords);
        if (tileData == null)
        {
            return false;
        }

        return tileData.GetCustomData(BLOCKED).AsBool();
    }

    public bool IsMouseOnBuildableTile()
    {
        Vector2I mapCoords = GetMouseTilePos();
        if (!_buildableTiles.Contains(mapCoords))
        {
            return false;
        }

        return !_occupiedTiles.Contains(mapCoords) && !_blockedTiles.Contains(mapCoords);
    }

    public void DestroyRandomBuildableTile()
    {
        var buildableTiles = new List<Vector2I>();
        foreach (Vector2I tilePos in _buildableTiles)
        {
            if (!_blockedTiles.Contains(tilePos))
            {
                buildableTiles.Add(tilePos);
            }
        }

        if (buildableTiles.Count == 0)
        {
            return;
        }

        int randIndex = (int)(GD.Randi() % (uint)buildableTiles.Count);
        Vector2I tileToBlock = buildableTiles[randIndex];
        _occupiedTiles.Add(tileToBlock);
        SetCell(tileToBlock, ATLAS_ID, NORMAL_TILE_POS);
        _buildableTiles.Remove(tileToBlock);
    }

    public Vector2 GetCurrentTilePos()
    {
        Vector2 centerPosLocal = MapToLocal(GetMouseTilePos());
        centerPosLocal.Y -= 16;
        return ToGlobal(centerPosLocal);
    }

    public void SetTileOccupied(Vector2I mapCoords)
    {
        _occupiedTiles.Add(mapCoords);
        _buildableTiles.Remove(mapCoords);
    }

    public void SetTileFree(Vector2I mapCoords)
    {
        if (_occupiedTiles.Contains(mapCoords))
        {
            _occupiedTiles.Remove(mapCoords);
            _buildableTiles.Add(mapCoords);
        }
    }

    public void UnblockTile(Vector2I mapCoords)
    {
        if (_blockedTiles.Count == 0)
        {
            return;
        }

        _blockedTiles.Remove(mapCoords);
        SetCell(mapCoords, ATLAS_ID, UNLOCK_TILE_POS);
        _buildableTiles.Add(mapCoords);
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
                _blockedTiles.Add(mapCoords);
            }
            else if (tileData.GetCustomData(BuildableCustomDataKey).AsBool())
            {
                _buildableTiles.Add(mapCoords);
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
        SetTileFree(tile);
    }
}
