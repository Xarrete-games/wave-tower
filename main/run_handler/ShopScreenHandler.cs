using Godot;
using System;
using System.Threading.Tasks;

public partial class ShopScreenHandler : Node
{
    private static readonly PackedScene ShopScreenScene = GD.Load<PackedScene>("uid://bpf44acq183yv");

    public event Action ShopClosed;

    public async Task OpenShopAsync(CanvasLayer eventLayer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        var relics = runContext.OffersManager.CreateRelicOffers(5);
        var consumables = runContext.OffersManager.CreateConsumablesOffers(5);

        ShopScreen shopScreen = ShopScreenScene.Instantiate<ShopScreen>();
        eventLayer.AddChild(shopScreen);
        shopScreen.SetRelics(relics);
        shopScreen.SetConsumables(consumables);
        shopScreen.ItemPurchase += OnItemPurchased;

        await ToSignal(shopScreen, Node.SignalName.TreeExited);
        shopScreen.ItemPurchase -= OnItemPurchased;
        ShopClosed?.Invoke();
    }

    private void OnItemPurchased(ItemOffer itemOffer)
    {
        var runContext = GetNode<RunContext>("/root/RunContext");
        runContext.OffersManager.PurchaseOffer(itemOffer);
    }
}