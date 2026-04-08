public sealed class ButterfishNigiri : RelicModel
{
    private const float _discountAmount = 0.1f;

    public ButterfishNigiri() : base("butterfish_nigiri")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        if (context.Type == PriceContext.PriceType.Consumable)
        {
            context.Discount += _discountAmount;
        }
    }
}
