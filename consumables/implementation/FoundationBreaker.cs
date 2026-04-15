public class FoundationBreaker : ConsumableTargeteable
{
    public override void action(object p_target)
    {
        CompositeTileMap tileMap = (CompositeTileMap)p_target;
        tileMap.unblock_tile_at_mouse();
    }
}
