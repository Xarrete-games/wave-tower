public class PriceContext
{
    public enum PriceType
    {
        Tower,
        Relic,
        Consumable,
    }

    public int price_type
    {
        get => (int)Type;
        set => Type = (PriceType)value;
    }

    public int base_price
    {
        get => BasePrice;
        set => BasePrice = value;
    }

    public float discount
    {
        get => Discount;
        set => Discount = value;
    }

    public int final_price
    {
        get => FinalPrice;
        set => FinalPrice = value;
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
