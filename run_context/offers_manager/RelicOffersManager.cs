using Godot;
using System.Collections.Generic;

public class RelicOffersManager
{
    private static readonly Godot.Collections.Dictionary<int, int> BASE_PRICE_BY_RARITY = new()
    {
        { 0, 50 },
        { 1, 80 },
        { 2, 120 },
    };

    private readonly List<RelicData> _allRelicData = new();

    public RelicOffersManager()
    {
        _allRelicData.AddRange(DataLoaderAccess.GetAllRelicsTyped());
    }

    public List<ItemOffer> get_relics_offers_by_ids(Godot.Collections.Array<string> relic_ids)
    {
        var offers = new List<ItemOffer>();

        for (int index = 0; index < relic_ids.Count; index++)
        {
            string relicId = relic_ids[index];
            for (int dataIndex = 0; dataIndex < _allRelicData.Count; dataIndex++)
            {
                RelicData data = _allRelicData[dataIndex];

                if (data.id == relicId)
                {
                    offers.Add(create_relic_offer_from_data(data));
                    break;
                }
            }
        }

        return offers;
    }

    public List<ItemOffer> create_relic_offers(int amount)
    {
        var filtered = new List<RelicData>();

        RunContext runContext = GetRunContext();
        RelicsManager relicsManager = runContext?.relics_manager;

        for (int index = 0; index < _allRelicData.Count; index++)
        {
            RelicData data = _allRelicData[index];

            bool hasRelic = relicsManager != null && relicsManager.has_relic(data.id);
            bool isCursed = data.IsCursed;
            bool onlyForEvents = data.OnlyForEvents;

            if (!hasRelic && !isCursed && !onlyForEvents)
            {
                filtered.Add(data);
            }
        }

        Shuffle(filtered);

        var offers = new List<ItemOffer>();
        for (int index = 0; index < filtered.Count && offers.Count < amount; index++)
        {
            offers.Add(create_relic_offer_from_data(filtered[index]));
        }

        return offers;
    }

    public ItemOffer create_relic_offer_from_data(RelicData data)
    {
        if (data == null)
        {
            return null;
        }

        int rarity = (int)data.rarity;
        int basePrice = BASE_PRICE_BY_RARITY.ContainsKey(rarity) ? BASE_PRICE_BY_RARITY[rarity] : BASE_PRICE_BY_RARITY[0];
        var ctx = new PriceContext(PriceContext.PriceType.Relic, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);

        int healthPrice = data.HealthPrice;
        return new ItemOffer(data, ctx.FinalPrice, healthPrice);
    }

    private RunContext GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
    }

    private void Shuffle(List<RelicData> items)
    {
        var rng = new RandomNumberGenerator();
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = rng.RandiRange(0, i);
            RelicData tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }
    }
}
