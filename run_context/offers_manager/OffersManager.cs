using Godot;
using System.Collections.Generic;

public class OffersManager
{
    private readonly RelicOffersManager _relicsOffersManager;
    private readonly ConsumablesOffersManager _consumablesOffersManager;

    public OffersManager()
    {
        _relicsOffersManager = new RelicOffersManager();
        _consumablesOffersManager = new ConsumablesOffersManager();
    }

    public List<ItemOffer> create_relic_offers(int amount)
    {
        return _relicsOffersManager.create_relic_offers(amount);
    }

    public ItemOffer create_relic_offer_from_data(RelicData data)
    {
        return _relicsOffersManager.create_relic_offer_from_data(data);
    }

    public List<ItemOffer> create_relic_offers_from_data(List<RelicData> data)
    {
        var offers = new List<ItemOffer>();
        for (int index = 0; index < data.Count; index++)
        {
            RelicData relicData = data[index];
            if (relicData != null)
            {
                offers.Add(_relicsOffersManager.create_relic_offer_from_data(relicData));
            }
        }

        return offers;
    }

    public List<ItemOffer> create_consumables_offers(int amount)
    {
        return _consumablesOffersManager.create_consumables_offers(amount);
    }

    public ItemOffer create_consumable_offer_from_data(ConsumableData data)
    {
        return _consumablesOffersManager.create_consumable_offer_from_data(data);
    }

    public void purchase_offer(ItemOffer itemOffer)
    {
        if (itemOffer == null)
        {
            return;
        }

        RunContext runContext = GetRunContext();
        if (runContext == null)
        {
            return;
        }

        Economy economy = runContext.economy;
        Status status = runContext.status;
        ConsumablesManager consumablesManager = runContext.consumables_manager;
        RelicsManager relicsManager = runContext.relics_manager;
        if (economy == null || status == null)
        {
            return;
        }

        int price = itemOffer.Price;
        int healthPrice = itemOffer.HealthPrice;

        economy.gold -= price;
        if (healthPrice > 0)
        {
            status.health -= healthPrice;
        }

        Resource itemData = itemOffer.ItemData;
        if (itemData is ConsumableData consumableData)
        {
            Consumable item = consumableData.CreateConsumable();
            if (item != null)
            {
                consumablesManager?.add_consumable(item);
            }
            return;
        }

        if (itemData is RelicData relicData)
        {
            Relic relic = relicData.CreateItem();
            relicsManager?.add_relic(relic);
            return;
        }
    }

    private RunContext GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
    }

}
