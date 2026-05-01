public class FoundationBreaker : ConsumableTargeteable {
    public override void Action(object target) {
        CompositeTileMap tileMap = (CompositeTileMap)target;
        tileMap.UnblockTileAtMouse();
    }
}

