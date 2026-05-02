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

    public List<ItemOffer> CreateRelicOffers(int amount)
    {
        return _relicsOffersManager.CreateRelicOffers(amount);
    }

    public ItemOffer CreateRelicOfferFromData(RelicData data)
    {
        return _relicsOffersManager.CreateRelicOfferFromData(data);
    }

    public List<ItemOffer> CreateRelicOffersFromData(List<RelicData> data)
    {
        var offers = new List<ItemOffer>();
        for (int index = 0; index < data.Count; index++)
        {
            RelicData relicData = data[index];
            if (relicData != null)
            {
                offers.Add(_relicsOffersManager.CreateRelicOfferFromData(relicData));
            }
        }

        return offers;
    }

    public List<ItemOffer> CreateConsumablesOffers(int amount)
    {
        return _consumablesOffersManager.CreateConsumablesOffers(amount);
    }

    public ItemOffer CreateConsumableOfferFromData(ConsumableData data)
    {
        return _consumablesOffersManager.CreateConsumableOfferFromData(data);
    }

    public void PurchaseOffer(ItemOffer itemOffer)
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

        Economy economy = runContext.Economy;
        Status status = runContext.Status;
        ConsumablesManager consumablesManager = runContext.ConsumablesManager;
        RelicsManager relicsManager = runContext.RelicsManager;
        if (economy == null || status == null)
        {
            return;
        }

        int price = itemOffer.Price;
        int healthPrice = itemOffer.HealthPrice;

        economy.Gold -= price;
        if (healthPrice > 0)
        {
            status.Health -= healthPrice;
        }

        Resource itemData = itemOffer.ItemData;
        if (itemData is ConsumableData consumableData)
        {
            Consumable item = consumableData.CreateConsumable();
            if (item != null)
            {
                consumablesManager?.AddConsumable(item);
            }
            return;
        }

        if (itemData is RelicData relicData)
        {
            Relic relic = relicData.CreateItem();
            relicsManager?.AddRelic(relic);
            return;
        }
    }

    private RunContext GetRunContext()
    {
        return RunContext.Instance;
    }

}
