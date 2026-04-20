public class PriceContext
{
    public enum PriceType
    {
        Tower,
        Relic,
        Consumable,
    }

    public PriceType Type { get; set; }
    public int BasePrice { get; set; }
    public float Discount { get; set; }
    public int FinalPrice { get; set; }

    public PriceContext(PriceType type, int basePrice)
    {
        Type = type;
        BasePrice = basePrice;
        Discount = 0f;
        FinalPrice = basePrice;
    }
}
