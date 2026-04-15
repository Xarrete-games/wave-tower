using Godot;
using System;
using System.Collections.Generic;

public partial class ShopScreen : Control
{
    public event Action<ItemOffer> item_purchase;

    private static readonly PackedScene ShopSlotScene = GD.Load<PackedScene>("uid://f428sxnliflm");

    [Export]
    public Control relics_container;

    [Export]
    public Control consumables_container;

    [Export]
    public Control sell_relics_container;

    [Export]
    public Control relics_section;

    [Export]
    public Control consumables_section;

    [Export]
    public Control sell_section;

    [Export]
    public Button exit_button;

    [Export]
    public XarretaButton sell_button;

    private bool _isOnSellMode;

    public override void _Ready()
    {
        this.ChangeToBuyMode();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (!runContext.economy.is_sell_active)
        {
            this.sell_button.Visible = false;
        }

        this.BuildRelicsForSale();
    }

    public void set_relics(List<ItemOffer> relics)
    {
        foreach (ItemOffer relic in relics)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.relics_container.AddChild(slot);
            slot.set_item(relic);
            slot.item_purchased += this.OnItemPurchase;
        }
    }

    public void set_consumables(List<ItemOffer> consumables)
    {
        foreach (ItemOffer consumable in consumables)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.consumables_container.AddChild(slot);
            slot.set_item(consumable);
            slot.item_purchased += this.OnItemPurchase;
        }
    }

    private void OnItemPurchase(ItemOffer itemOffer, ShopSlot slotPurchased)
    {
        this.item_purchase?.Invoke(itemOffer);
        GetNode<AudioManager>("/root/AudioManager").play_purchase();

        Resource itemData = itemOffer?.item_data;
        string purchasedId = itemData switch
        {
            RelicData relicData => relicData.id,
            ConsumableData consumableData => consumableData.id,
            _ => string.Empty,
        };

        foreach (Node slot in this.relics_container.GetChildren())
        {
            if (slot == slotPurchased)
            {
                if (purchasedId == "strategy_tome_economy")
                {
                    this.sell_button.Visible = true;
                }

                slot.QueueFree();
                return;
            }
        }

        foreach (Node slot in this.consumables_container.GetChildren())
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
        foreach (Node slot in this.sell_relics_container.GetChildren())
        {
            if (slot != slotSold)
            {
                continue;
            }

            slot.QueueFree();

            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            Resource itemData = itemOffer?.item_data;
            if (itemData is RelicData relicData)
            {
                runContext.relics_manager.remove_relic(relicData.id);
            }

            runContext.economy.add_gold(itemOffer?.price ?? 0);
            GetNode<AudioManager>("/root/AudioManager").play_purchase();
            this.sell_button.disable();
            this._on_exit_button_pressed();
            return;
        }
    }

    private void _on_exit_button_pressed()
    {
        if (this._isOnSellMode)
        {
            this.ChangeToBuyMode();
            return;
        }

        QueueFree();
    }

    private void _on_sell_button_pressed()
    {
        this.ChangeToSellMode();
    }

    private void ChangeToSellMode()
    {
        this._isOnSellMode = true;
        this.sell_button.Visible = false;
        this.exit_button.Visible = true;
        this.relics_section.Visible = false;
        this.consumables_section.Visible = false;
        this.sell_section.Visible = true;
    }

    private void ChangeToBuyMode()
    {
        this.sell_button.Visible = true;
        this._isOnSellMode = false;
        this.relics_section.Visible = true;
        this.consumables_section.Visible = true;
        this.sell_section.Visible = false;
    }

    private void BuildRelicsForSale()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        var currentRelics = runContext.relics_manager.get_all_relics();
        var currentRelicsData = new List<RelicData>();
        foreach (Relic relic in currentRelics)
        {
            if (relic?.Data != null)
            {
                currentRelicsData.Add(relic.Data);
            }
        }

        var relicOffers = runContext.offers_manager.create_relic_offers_from_data(currentRelicsData);
        foreach (ItemOffer relicOffer in relicOffers)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.sell_relics_container.AddChild(slot);
            slot.set_item(relicOffer);
            slot.item_purchased += this.OnItemSold;
        }
    }
}