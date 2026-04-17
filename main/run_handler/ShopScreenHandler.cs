using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopScreenHandler : Node
{
    private static readonly PackedScene ShopScreenScene = GD.Load<PackedScene>("uid://bpf44acq183yv");

    public event Action shop_closed;

    public async Task OpenShopAsync(CanvasLayer eventLayer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        var relics = runContext.offers_manager.create_relic_offers(5);
        var consumables = runContext.offers_manager.create_consumables_offers(5);

        ShopScreen shopScreen = ShopScreenScene.Instantiate<ShopScreen>();
        eventLayer.AddChild(shopScreen);
        shopScreen.set_relics(relics);
        shopScreen.set_consumables(consumables);
        shopScreen.item_purchase += OnItemPurchased;

        await ToSignal(shopScreen, Node.SignalName.TreeExited);
        shopScreen.item_purchase -= OnItemPurchased;
        shop_closed?.Invoke();
    }

    private void OnItemPurchased(ItemOffer itemOffer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.offers_manager.purchase_offer(itemOffer);
    }
}