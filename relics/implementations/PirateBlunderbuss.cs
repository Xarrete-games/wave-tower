public sealed class PirateBlunderbuss : Relic
{
    public PirateBlunderbuss() : base("pirate_blunderbuss")
    {
    }

    public override void OnBeforeGetLoot(LootContext context)
    {
        context.ChanceDropConsumable += 20;
    }
}
