public sealed class SoyaSauce : Relic
{

    public SoyaSauce() : base("soya_sauce")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        float discountBefore = context.Discount;
        bool hasTuna = this.HasRelicById("tuna_nigiri");
        bool hasSalmon = this.HasRelicById("salmon_nigiri");
        bool hasButterfish = this.HasRelicById("butterfish_nigiri");

        switch (context.Type)
        {
            case PriceContext.PriceType.Tower:
                if (hasTuna)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Relic:
                if (hasSalmon)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Consumable:
                if (hasButterfish)
                {
                    context.Discount += 0.1f;
                }
                break;
        }
    }

    private bool HasRelicById(string relicId)
    {
        return RunContextRuntime.RelicsManager.HasRelic(relicId);
    }
}
