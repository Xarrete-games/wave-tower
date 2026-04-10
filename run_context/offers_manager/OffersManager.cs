using Godot;

[GlobalClass]
public partial class OffersManager : RefCounted
{
    private readonly RelicOffersManager _relicsOffersManager;
    private readonly ConsumablesOffersManager _consumablesOffersManager;

    public OffersManager()
    {
        this._relicsOffersManager = new RelicOffersManager();
        this._consumablesOffersManager = new ConsumablesOffersManager();
    }

    public Godot.Collections.Array<Variant> create_relic_offers(int amount)
    {
        return this._relicsOffersManager.create_relic_offers(amount);
    }

    public Variant create_relic_offer_from_data(Variant data)
    {
        return this._relicsOffersManager.create_relic_offer_from_data(data);
    }

    public Godot.Collections.Array<Variant> create_relic_offers_from_data(Godot.Collections.Array<Variant> data)
    {
        var offers = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < data.Count; index++)
        {
            offers.Add(this._relicsOffersManager.create_relic_offer_from_data(data[index]));
        }

        return offers;
    }

    public Godot.Collections.Array<Variant> create_consumables_offers(int amount)
    {
        return this._consumablesOffersManager.create_consumables_offers(amount);
    }

    public Variant create_consumable_offer_from_data(Variant data)
    {
        return this._consumablesOffersManager.create_consumable_offer_from_data(data);
    }

    public Variant purchase_offer(Variant itemOfferVariant)
    {
        GodotObject itemOffer = itemOfferVariant.AsGodotObject();
        if (itemOffer == null)
        {
            return default;
        }

        GodotObject runContext = this.GetRunContext();
        if (runContext == null)
        {
            return default;
        }

        GodotObject economy = runContext.Get("economy").AsGodotObject();
        GodotObject status = runContext.Get("status").AsGodotObject();
        GodotObject consumablesManager = runContext.Get("consumables_manager").AsGodotObject();
        GodotObject relicsManager = runContext.Get("relics_manager").AsGodotObject();

        int price = (int)itemOffer.Get("price");
        int healthPrice = (int)itemOffer.Get("health_price");

        economy.Set("gold", (int)economy.Get("gold") - price);
        if (healthPrice > 0)
        {
            status.Set("health", (int)status.Get("health") - healthPrice);
        }

        GodotObject itemData = itemOffer.Get("item_data").AsGodotObject();
        Variant item = itemData.Call("create_item");

        bool isConsumable = this.HasProperty(itemData, "consumable_type");
        if (isConsumable)
        {
            consumablesManager.Call("add_consumable", item);
        }
        else
        {
            relicsManager.Call("add_relic", item);
        }

        return item;
    }

    private GodotObject GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<Node>("/root/RunContext");
    }

    private bool HasProperty(GodotObject obj, string propertyName)
    {
        if (obj == null || string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        Godot.Collections.Array<Godot.Collections.Dictionary> propertyList = obj.GetPropertyList();
        for (int index = 0; index < propertyList.Count; index++)
        {
            string name = propertyList[index].ContainsKey("name") ? propertyList[index]["name"].AsString() : string.Empty;
            if (name == propertyName)
            {
                return true;
            }
        }

        return false;
    }
}
