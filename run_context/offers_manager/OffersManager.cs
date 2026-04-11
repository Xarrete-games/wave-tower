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
        ItemOffer itemOffer = itemOfferVariant.AsGodotObject() as ItemOffer;
        if (itemOffer == null)
        {
            return default;
        }

        RunContext runContext = this.GetRunContext();
        if (runContext == null)
        {
            return default;
        }

        Economy economy = runContext.economy;
        Status status = runContext.status;
        ConsumablesManager consumablesManager = runContext.consumables_manager;
        RelicsManager relicsManager = runContext.relics_manager;
        if (economy == null || status == null)
        {
            return default;
        }

        int price = itemOffer.price;
        int healthPrice = itemOffer.health_price;

        economy.gold -= price;
        if (healthPrice > 0)
        {
            status.health -= healthPrice;
        }

        GodotObject itemData = itemOffer.item_data.AsGodotObject();
        Variant item = this.CreateItemFromData(itemData);

        bool isConsumable = this.HasProperty(itemData, "consumable_type");
        if (isConsumable)
        {
            consumablesManager?.add_consumable(item);
        }
        else
        {
            relicsManager?.add_relic(item);
        }

        return item;
    }

    private Variant CreateItemFromData(GodotObject itemData)
    {
        if (itemData == null)
        {
            return default;
        }

        if (itemData is ConsumableData consumableData)
        {
            return consumableData.create_item();
        }

        if (itemData is RelicData relicData)
        {
            return relicData.create_item();
        }

        if (itemData.HasMethod("create_item"))
        {
            return itemData.Call("create_item");
        }

        return default;
    }

    private RunContext GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
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
