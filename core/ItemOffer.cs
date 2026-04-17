using Godot;

public class ItemOffer
{
    public Resource item_data { get; set; }

    public int price { get; set; }

    public int health_price { get; set; }

    public ItemOffer()
    {
    }

    public ItemOffer(Resource itemData, int itemPrice, int healthPrice = 0)
    {
        item_data = itemData;
        price = itemPrice;
        health_price = healthPrice;
    }
}