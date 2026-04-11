using Godot;

[GlobalClass]
public partial class RelicOffersManager : RefCounted
{
    private static readonly Godot.Collections.Dictionary<int, int> BASE_PRICE_BY_RARITY = new()
    {
        { 0, 50 },
        { 1, 80 },
        { 2, 120 },
    };

    private Godot.Collections.Array<Variant> _allRelicData = new();

    public RelicOffersManager()
    {
        this._allRelicData = DataLoaderAccess.GetAllRelics();
    }

    public Godot.Collections.Array<Variant> get_relics_offers_by_ids(Godot.Collections.Array<string> relic_ids)
    {
        var offers = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < relic_ids.Count; index++)
        {
            string relicId = relic_ids[index];
            for (int dataIndex = 0; dataIndex < this._allRelicData.Count; dataIndex++)
            {
                GodotObject data = this._allRelicData[dataIndex].AsGodotObject();
                if (data == null)
                {
                    continue;
                }

                if ((string)data.Get("id") == relicId)
                {
                    offers.Add(this.create_relic_offer_from_data(this._allRelicData[dataIndex]));
                    break;
                }
            }
        }

        return offers;
    }

    public Godot.Collections.Array<Variant> create_relic_offers(int amount)
    {
        var filtered = new Godot.Collections.Array<Variant>();

        RunContext runContext = this.GetRunContext();
        RelicsManager relicsManager = runContext?.relics_manager;

        for (int index = 0; index < this._allRelicData.Count; index++)
        {
            GodotObject data = this._allRelicData[index].AsGodotObject();
            if (data == null)
            {
                continue;
            }

            bool hasRelic = relicsManager != null && relicsManager.has_relic((string)data.Get("id"));
            bool isCursed = (bool)data.Get("is_cursed");
            bool onlyForEvents = (bool)data.Get("only_for_events");

            if (!hasRelic && !isCursed && !onlyForEvents)
            {
                filtered.Add(this._allRelicData[index]);
            }
        }

        this.Shuffle(filtered);

        var offers = new Godot.Collections.Array<Variant>();
        for (int index = 0; index < filtered.Count && offers.Count < amount; index++)
        {
            offers.Add(this.create_relic_offer_from_data(filtered[index]));
        }

        return offers;
    }

    public Variant create_relic_offer_from_data(Variant dataVariant)
    {
        GodotObject data = dataVariant.AsGodotObject();
        if (data == null)
        {
            return default;
        }

        int rarity = (int)data.Get("rarity");
        int basePrice = BASE_PRICE_BY_RARITY.ContainsKey(rarity) ? BASE_PRICE_BY_RARITY[rarity] : BASE_PRICE_BY_RARITY[0];
        var ctx = new PriceContext(PriceContext.PriceType.Relic, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);

        int healthPrice = (int)data.Get("health_price");
        return new ItemOffer(dataVariant, ctx.FinalPrice, healthPrice);
    }

    private RunContext GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
    }

    private void Shuffle(Godot.Collections.Array<Variant> items)
    {
        var rng = new RandomNumberGenerator();
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = rng.RandiRange(0, i);
            Variant tmp = items[i];
            items[i] = items[j];
            items[j] = tmp;
        }
    }
}
