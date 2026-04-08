public sealed class IceCream : RelicModel
{
    public IceCream() : base("ice_cream")
    {
    }

    public override void OnBeforeGetLoot(LootContext context)
    {
        context.ExtraGold += 10;
    }
}
