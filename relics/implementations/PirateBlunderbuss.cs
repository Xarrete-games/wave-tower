public sealed class PirateBlunderbuss : RelicModel
{
    public PirateBlunderbuss() : base("pirate_blunderbuss")
    {
    }

    public override void OnBeforeGetLoot(LootContext context)
    {
        context.ChanceDropConsumable += 20;
    }
}
