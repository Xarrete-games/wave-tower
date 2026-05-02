using Godot;

public class FoundationBreaker : ConsumableTargeteable {
    public override void Action(Node target) {
        CompositeTileMap tileMap = (CompositeTileMap)target;
        tileMap.UnblockTileAtMouse();
    }
}

