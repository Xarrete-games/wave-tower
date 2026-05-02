using Godot;
using System.Collections.Generic;

public class RelicOffersManager
{
    private static readonly Dictionary<int, int> BasePriceByRarity = new()
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

    public List<ItemOffer> GetRelicsOffersByIds(IReadOnlyList<string> relicIds)
    {
        var offers = new List<ItemOffer>();

        for (int index = 0; index < relicIds.Count; index++)
        {
            string relicId = relicIds[index];
            for (int dataIndex = 0; dataIndex < _allRelicData.Count; dataIndex++)
            {
                RelicData data = _allRelicData[dataIndex];

                if (data.Id == relicId)
                {
                    offers.Add(CreateRelicOfferFromData(data));
                    break;
                }
            }
        }

        return offers;
    }

    public List<ItemOffer> CreateRelicOffers(int amount)
    {
        var filtered = new List<RelicData>();

        RunContext runContext = GetRunContext();
        RelicsManager relicsManager = runContext?.RelicsManager;

        for (int index = 0; index < _allRelicData.Count; index++)
        {
            RelicData data = _allRelicData[index];

            bool hasRelic = relicsManager != null && relicsManager.HasRelic(data.Id);
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
            offers.Add(CreateRelicOfferFromData(filtered[index]));
        }

        return offers;
    }

    public ItemOffer CreateRelicOfferFromData(RelicData data)
    {
        if (data == null)
        {
            return null;
        }

        int rarity = (int)data.Rarity;
        int basePrice = BasePriceByRarity.ContainsKey(rarity) ? BasePriceByRarity[rarity] : BasePriceByRarity[0];
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
