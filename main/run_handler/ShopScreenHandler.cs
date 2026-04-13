using Godot;
using System.Threading.Tasks;

public partial class ShopScreenHandler : Node
{
    private static readonly PackedScene ShopScreenScene = GD.Load<PackedScene>("uid://bpf44acq183yv");

    [Signal]
    public delegate void shop_closedEventHandler();

    public async Task OpenShopAsync(CanvasLayer eventLayer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        var relics = runContext.offers_manager.create_relic_offers(5);
        var consumables = runContext.offers_manager.create_consumables_offers(5);

        ShopScreen shopScreen = ShopScreenScene.Instantiate<ShopScreen>();
        eventLayer.AddChild(shopScreen);
        shopScreen.set_relics(relics);
        shopScreen.set_consumables(consumables);
        shopScreen.item_purchase += this.OnItemPurchased;

        await ToSignal(shopScreen, "tree_exited");
        shopScreen.item_purchase -= this.OnItemPurchased;
        EmitSignal(SignalName.shop_closed);
    }

    private void OnItemPurchased(ItemOffer itemOffer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.offers_manager.purchase_offer(itemOffer);
    }
}