using Godot;

public class ItemOffer
{
    public Resource ItemData { get; set; }

    public int Price { get; set; }

    public int HealthPrice { get; set; }

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