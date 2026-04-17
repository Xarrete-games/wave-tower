
using System.Collections.Generic;

public class ConsumablesOffersManager
{
    private static readonly Godot.Collections.Dictionary<int, int> BASE_PRICE_BY_RARITY = new()
    {
        { 0, 50 },
        { 1, 80 },
        { 2, 120 },
    };

    private readonly List<ConsumableData> _allConsumablesData = new();

    public ConsumablesOffersManager()
    {
        _allConsumablesData.AddRange(DataLoaderAccess.GetAllConsumablesTyped());
    }

    public List<ItemOffer> create_consumables_offers(int amount)
    {
        var consumablesData = new List<ConsumableData>(_allConsumablesData);
        var offers = new List<ItemOffer>();

        for (int index = 0; index < consumablesData.Count && offers.Count < amount; index++)
        {
            offers.Add(create_consumable_offer_from_data(consumablesData[index]));
        }

        return offers;
    }

    public ItemOffer create_consumable_offer_from_data(ConsumableData data)
    {
        if (data == null)
        {
            return null;
        }

        int rarity = (int)data.rarity;
        int basePrice = BASE_PRICE_BY_RARITY.ContainsKey(rarity) ? BASE_PRICE_BY_RARITY[rarity] : BASE_PRICE_BY_RARITY[0];
        var ctx = new PriceContext(PriceContext.PriceType.Consumable, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);

        return new ItemOffer(data, ctx.FinalPrice, 0);
    }
}
