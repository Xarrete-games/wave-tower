public sealed class IceCream : Relic
{
    public IceCream() : base("ice_cream")
    {
    }

    public override void OnBeforeGetLoot(LootContext context)
    {
        context.ExtraGold += 10;
    }
}
