public sealed class TunaNigiri : Relic
{
    public TunaNigiri() : base("tuna_nigiri")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        if (context.Type == PriceContext.PriceType.Tower)
        {
            context.Discount += 0.1f;
        }
    }
}
