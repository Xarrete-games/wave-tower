using Godot;

public class ItemOffer
{
    public Resource ItemData { get; set; }

    public int Price { get; set; }

    public int HealthPrice { get; set; }

    // Legacy aliases kept temporarily while migrating remaining consumers.
    public Resource item_data
    {
        get => ItemData;
        set => ItemData = value;
    }

    public int price
    {
        get => Price;
        set => Price = value;
    }

    public int health_price
    {
        get => HealthPrice;
        set => HealthPrice = value;
    }

    public ItemOffer()
    {
    }

    public ItemOffer(Resource itemData, int itemPrice, int healthPrice = 0)
    {
        ItemData = itemData;
        Price = itemPrice;
        HealthPrice = healthPrice;
    }
}