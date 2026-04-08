public sealed class PriceContext
{
    public enum PriceType
    {
        Tower,
        Relic,
        Consumable,
    }

    public PriceType Type { get; }
    public int BasePrice { get; }
    public float Discount { get; set; }
    public int FinalPrice { get; set; }

    public PriceContext(PriceType type, int basePrice)
    {
        this.Type = type;
        this.BasePrice = basePrice;
        this.Discount = 0f;
        this.FinalPrice = basePrice;
    }
}
