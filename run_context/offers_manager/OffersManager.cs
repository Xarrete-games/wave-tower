using Godot;
using System.Collections.Generic;

public class OffersManager
{
    private readonly RelicOffersManager _relicsOffersManager;
    private readonly ConsumablesOffersManager _consumablesOffersManager;

    public OffersManager()
    {
        this._relicsOffersManager = new RelicOffersManager();
        this._consumablesOffersManager = new ConsumablesOffersManager();
    }

    public List<ItemOffer> create_relic_offers(int amount)
    {
        return this._relicsOffersManager.create_relic_offers(amount);
    }

    public ItemOffer create_relic_offer_from_data(Variant data)
    {
        return this._relicsOffersManager.create_relic_offer_from_data(data);
    }

    public List<ItemOffer> create_relic_offers_from_data(Godot.Collections.Array<Variant> data)
    {
        var offers = new List<ItemOffer>();
        for (int index = 0; index < data.Count; index++)
        {
            offers.Add(this._relicsOffersManager.create_relic_offer_from_data(data[index]));
        }

        return offers;
    }

    public List<ItemOffer> create_consumables_offers(int amount)
    {
        return this._consumablesOffersManager.create_consumables_offers(amount);
    }

    public ItemOffer create_consumable_offer_from_data(Variant data)
    {
        return this._consumablesOffersManager.create_consumable_offer_from_data(data);
    }

    public Variant purchase_offer(ItemOffer itemOffer)
    {
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
        bool isConsumable = this.HasProperty(itemData, "consumable_type");
        if (isConsumable)
        {
            Variant item = this.CreateItemFromData(itemData);
            consumablesManager?.add_consumable(item);
            return item;
        }

        if (itemData is RelicData relicData)
        {
            Relic relic = relicData.create_item();
            relicsManager?.add_relic(relic);
            return Variant.From(relicData.id);
        }

        return default;
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
            return Variant.From(relicData.id);
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
