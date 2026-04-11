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
        var relics = runContext.offers_manager.Call("create_relic_offers", 5).AsGodotArray<Variant>();
        var consumables = runContext.offers_manager.Call("create_consumables_offers", 5).AsGodotArray<Variant>();

        Node shopScreen = ShopScreenScene.Instantiate();
        eventLayer.AddChild(shopScreen);
        shopScreen.Call("set_relics", relics);
        shopScreen.Call("set_consumables", consumables);
        shopScreen.Connect("item_purchase", Callable.From<Variant>(this.OnItemPurchased));

        await ToSignal(shopScreen, "tree_exited");
        EmitSignal(SignalName.shop_closed);
    }

    private void OnItemPurchased(Variant itemOffer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.offers_manager.Call("purchase_offer", itemOffer);
    }
}