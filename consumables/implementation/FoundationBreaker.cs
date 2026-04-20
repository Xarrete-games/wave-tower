public class FoundationBreaker : ConsumableTargeteable {
    public override void Action(object target) {
        CompositeTileMap tileMap = (CompositeTileMap)target;
        tileMap.unblock_tile_at_mouse();
    }
}

