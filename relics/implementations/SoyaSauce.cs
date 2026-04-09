public sealed class SoyaSauce : RelicModel
{
    public bool HasTunaNigiri { get; set; }
    public bool HasSalmonNigiri { get; set; }
    public bool HasButterfishNigiri { get; set; }

    public SoyaSauce() : base("soya_sauce")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        switch (context.Type)
        {
            case PriceContext.PriceType.Tower:
                if (this.HasTunaNigiri)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Relic:
                if (this.HasSalmonNigiri)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Consumable:
                if (this.HasButterfishNigiri)
                {
                    context.Discount += 0.1f;
                }
                break;
        }
    }
}
