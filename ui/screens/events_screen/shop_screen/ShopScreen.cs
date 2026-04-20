using Godot;
using System;
using System.Collections.Generic;

public partial class ShopScreen : Control
{
    public event Action<ItemOffer> ItemPurchase;

    private static readonly PackedScene ShopSlotScene = GD.Load<PackedScene>("uid://f428sxnliflm");

    [Export]
    public Control RelicsContainer;

    [Export]
    public Control ConsumablesContainer;

    [Export]
    public Control SellRelicsContainer;

    [Export]
    public Control RelicsSection;

    [Export]
    public Control ConsumablesSection;

    [Export]
    public Control SellSection;

    [Export]
    public Button ExitButton;

    [Export]
    public XarretaButton SellButton;

    private bool _isOnSellMode;

    public override void _Ready()
    {
        ChangeToBuyMode();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (!runContext.economy.is_sell_active)
        {
            SellButton.Visible = false;
        }

        BuildRelicsForSale();
    }

    public void SetRelics(List<ItemOffer> relics)
    {
        foreach (ItemOffer relic in relics)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            RelicsContainer.AddChild(slot);
            slot.SetItem(relic);
            slot.ItemPurchased += OnItemPurchase;
        }
    }

    public void SetConsumables(List<ItemOffer> consumables)
    {
        foreach (ItemOffer consumable in consumables)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            ConsumablesContainer.AddChild(slot);
            slot.SetItem(consumable);
            slot.ItemPurchased += OnItemPurchase;
        }
    }

    private void OnItemPurchase(ItemOffer itemOffer, ShopSlot slotPurchased)
    {
        ItemPurchase?.Invoke(itemOffer);
        GetNode<AudioManager>("/root/AudioManager").play_purchase();

        Resource itemData = itemOffer?.ItemData;
        string purchasedId = itemData switch
        {
            RelicData relicData => relicData.Id,
            ConsumableData consumableData => consumableData.Id,
            _ => string.Empty,
        };

        foreach (Node slot in RelicsContainer.GetChildren())
        {
            if (slot == slotPurchased)
            {
                if (purchasedId == "strategy_tome_economy")
                {
                    SellButton.Visible = true;
                }

                slot.QueueFree();
                return;
            }
        }

        foreach (Node slot in ConsumablesContainer.GetChildren())
        {
            if (slot == slotPurchased)
            {
                slot.QueueFree();
                return;
            }
        }
    }

    private void OnItemSold(ItemOffer itemOffer, ShopSlot slotSold)
    {
        foreach (Node slot in SellRelicsContainer.GetChildren())
        {
            if (slot != slotSold)
            {
                continue;
            }

            slot.QueueFree();

            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            Resource itemData = itemOffer?.ItemData;
            if (itemData is RelicData relicData)
            {
                runContext.relics_manager.RemoveRelic(relicData.Id);
            }

            runContext.economy.AddGold(itemOffer?.Price ?? 0);
            GetNode<AudioManager>("/root/AudioManager").play_purchase();
            SellButton.disable();
            OnExitButtonPressed();
            return;
        }
    }

    private void OnExitButtonPressed()
    {
        if (_isOnSellMode)
        {
            ChangeToBuyMode();
            return;
        }

        QueueFree();
    }

    private void OnSellButtonPressed()
    {
        ChangeToSellMode();
    }

    private void ChangeToSellMode()
    {
        _isOnSellMode = true;
        SellButton.Visible = false;
        ExitButton.Visible = true;
        RelicsSection.Visible = false;
        ConsumablesSection.Visible = false;
        SellSection.Visible = true;
    }

    private void ChangeToBuyMode()
    {
        SellButton.Visible = true;
        _isOnSellMode = false;
        RelicsSection.Visible = true;
        ConsumablesSection.Visible = true;
        SellSection.Visible = false;
    }

    private void BuildRelicsForSale()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        var currentRelics = runContext.relics_manager.GetAllRelics();
        var currentRelicsData = new List<RelicData>();
        foreach (Relic relic in currentRelics)
        {
            if (relic?.Data != null)
            {
                currentRelicsData.Add(relic.Data);
            }
        }

        var relicOffers = runContext.offers_manager.CreateRelicOffersFromData(currentRelicsData);
        foreach (ItemOffer relicOffer in relicOffers)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            SellRelicsContainer.AddChild(slot);
            slot.SetItem(relicOffer);
            slot.ItemPurchased += OnItemSold;
        }
    }
}

