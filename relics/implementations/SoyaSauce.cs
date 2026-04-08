public sealed class SoyaSauce : RelicModel
{
    public SoyaSauce() : base("soya_sauce")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        switch (context.Type)
        {
            case PriceContext.PriceType.Tower:
                if (this.HasRelic("tuna_nigiri"))
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Relic:
                if (this.HasRelic("salmon_nigiri"))
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Consumable:
                if (this.HasRelic("butterfish_nigiri"))
                {
                    context.Discount += 0.1f;
                }
                break;
        }
    }
}
