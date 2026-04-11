using Godot;

[GlobalClass]
public partial class ItemOffer : RefCounted
{
    public Variant item_data { get; set; }

    public int price { get; set; }

    public int health_price { get; set; }

    public ItemOffer()
    {
    }

    public ItemOffer(Variant p_item_data, int p_price, int p_health_price = 0)
    {
        this.item_data = p_item_data;
        this.price = p_price;
        this.health_price = p_health_price;
    }
}