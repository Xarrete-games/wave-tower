using Godot;

[GlobalClass]
public partial class ConsumablesOffersManager : RefCounted
{
    private static readonly Godot.Collections.Dictionary<int, int> BASE_PRICE_BY_RARITY = new()
    {
        { 0, 50 },
        { 1, 80 },
        { 2, 120 },
    };

    private Godot.Collections.Array<Variant> _allConsumablesData = new();

    public ConsumablesOffersManager()
    {
        this._allConsumablesData = DataLoaderAccess.GetAllConsumables();
    }

    public Godot.Collections.Array<Variant> create_consumables_offers(int amount)
    {
        var consumablesData = this._allConsumablesData.Duplicate();
        var offers = new Godot.Collections.Array<Variant>();

        for (int index = 0; index < consumablesData.Count && offers.Count < amount; index++)
        {
            offers.Add(this.create_consumable_offer_from_data(consumablesData[index]));
        }

        return offers;
    }

    public Variant create_consumable_offer_from_data(Variant dataVariant)
    {
        GodotObject data = dataVariant.AsGodotObject();
        if (data == null)
        {
            return default;
        }

        int rarity = (int)data.Get("rarity");
        int basePrice = BASE_PRICE_BY_RARITY.ContainsKey(rarity) ? BASE_PRICE_BY_RARITY[rarity] : BASE_PRICE_BY_RARITY[0];
        var ctx = new PriceContext(PriceContext.PriceType.Consumable, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);

        return new ItemOffer(dataVariant, ctx.FinalPrice, 0);
    }
}
