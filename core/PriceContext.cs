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
        get => (int)this.Type;
        set => this.Type = (PriceType)value;
    }

    public int base_price
    {
        get => this.BasePrice;
        set => this.BasePrice = value;
    }

    public float discount
    {
        get => this.Discount;
        set => this.Discount = value;
    }

    public int final_price
    {
        get => this.FinalPrice;
        set => this.FinalPrice = value;
    }

    public PriceType Type { get; set; }
    public int BasePrice { get; set; }
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
