using Godot;

public partial class ShopScreen : Control
{
    [Signal]
    public delegate void item_purchaseEventHandler(Variant item);

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
    public Button sell_button;

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

    public void set_relics(Godot.Collections.Array<Variant> relics)
    {
        foreach (Variant relic in relics)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.relics_container.AddChild(slot);
            slot.set_item(relic);
            slot.item_purchased += this.OnItemPurchase;
        }
    }

    public void set_consumables(Godot.Collections.Array<Variant> consumables)
    {
        foreach (Variant consumable in consumables)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.consumables_container.AddChild(slot);
            slot.set_item(consumable);
            slot.item_purchased += this.OnItemPurchase;
        }
    }

    private void OnItemPurchase(Variant itemOffer, Variant slotPurchased)
    {
        EmitSignal(SignalName.item_purchase, itemOffer);
        GetNode<Node>("/root/AudioManager").Call("play_purchase");

        GodotObject itemOfferObj = itemOffer.AsGodotObject();
        string purchasedId = itemOfferObj?.Get("item_data").AsGodotObject()?.Get("id").AsString();

        foreach (Node slot in this.relics_container.GetChildren())
        {
            if (slot == slotPurchased.AsGodotObject())
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
            if (slot == slotPurchased.AsGodotObject())
            {
                slot.QueueFree();
                return;
            }
        }
    }

    private void OnItemSold(Variant itemOffer, Variant slotSold)
    {
        foreach (Node slot in this.sell_relics_container.GetChildren())
        {
            if (slot != slotSold.AsGodotObject())
            {
                continue;
            }

            slot.QueueFree();

            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            GodotObject itemOfferObj = itemOffer.AsGodotObject();
            GodotObject itemData = itemOfferObj?.Get("item_data").AsGodotObject();
            if (itemData != null)
            {
                runContext.relics_manager.remove_relic((string)itemData.Get("id"));
            }

            runContext.economy.add_gold((int)(itemOfferObj?.Get("price") ?? 0));
            GetNode<Node>("/root/AudioManager").Call("play_purchase");
            this.sell_button.Call("disable");
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
        var currentRelicsData = new Godot.Collections.Array<Variant>();
        foreach (Relic relic in currentRelics)
        {
            if (relic?.Data != null)
            {
                currentRelicsData.Add(relic.Data);
            }
        }

        var relicOffers = runContext.offers_manager.create_relic_offers_from_data(currentRelicsData);
        foreach (Variant relicOffer in relicOffers)
        {
            ShopSlot slot = ShopSlotScene.Instantiate<ShopSlot>();
            this.sell_relics_container.AddChild(slot);
            slot.set_item(relicOffer);
            slot.item_purchased += this.OnItemSold;
        }
    }
}