public sealed class SalmonNigiri : RelicModel
{
    public SalmonNigiri() : base("salmon_nigiri")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        if (context.Type == PriceContext.PriceType.Relic)
        {
            context.Discount += 0.1f;
        }
    }
}
