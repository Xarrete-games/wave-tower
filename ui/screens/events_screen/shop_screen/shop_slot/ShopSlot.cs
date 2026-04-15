using Godot;
using System;

public partial class ShopSlot : VBoxContainer
{
    public event Action<ItemOffer, ShopSlot> item_purchased;

    [Export]
    public Label title_label;

    [Export]
    public RichTextLabel description_label;

    [Export]
    public Control gold_price;

    [Export]
    public ShopSlotIcon shop_slot_icon;

    private Control _healthPrice;
    private RunContext _runContext;
    private ItemOffer _item;
    private int _price;
    private bool _hasEnoughHealth;
    private int _currentHealthCost;

    public override void _Ready()
    {
        this._healthPrice = GetNode<Control>("HealthPrice");
        this._healthPrice.Visible = false;

        this._runContext = GetNode<RunContext>("/root/RunContext");
        if (this._runContext?.relics_manager != null)
        {
            this._runContext.relics_manager.relic_added += this._on_relic_added;
        }

        if (this._runContext?.status != null)
        {
            this._runContext.status.health_change += this._on_status_health_change;
        }
    }

    public override void _ExitTree()
    {
        if (this._runContext?.relics_manager != null)
        {
            this._runContext.relics_manager.relic_added -= this._on_relic_added;
        }

        if (this._runContext?.status != null)
        {
            this._runContext.status.health_change -= this._on_status_health_change;
        }
    }

    public void set_item(ItemOffer itemOffer)
    {
        if (itemOffer == null)
        {
            return;
        }

        this._item = itemOffer;
        this._price = itemOffer.price;

        Resource itemData = itemOffer.item_data;
        if (itemData == null)
        {
            return;
        }

        string displayName = string.Empty;
        string description = string.Empty;
        Texture2D icon = null;
        if (itemData is RelicData relicInfo)
        {
            displayName = relicInfo.display_name;
            description = relicInfo.description;
            icon = relicInfo.icon;
        }
        else if (itemData is ConsumableData consumableInfo)
        {
            displayName = consumableInfo.display_name;
            description = consumableInfo.description;
            icon = consumableInfo.icon;
        }

        this.title_label.Text = displayName;
        this.description_label.Text = description;
        this.TooltipText = description;
        this.gold_price?.Set("price", this._price);
        this.shop_slot_icon?.set_icon(icon);

        RelicData relicData = itemData as RelicData;
        if (relicData != null)
        {
            this.shop_slot_icon?.set_background_color(this._runContext.relics_manager.get_rarity_color(relicData.rarity));
        }

        this._currentHealthCost = itemOffer.health_price;
        this._healthPrice.Visible = this._currentHealthCost > 0;
        if (this._currentHealthCost > 0)
        {
            this._healthPrice.Set("price", this._currentHealthCost);
        }

        this._chek_health(this._runContext.status.health, this._currentHealthCost);
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (this._item == null)
        {
            return;
        }

        Resource itemData = this._item.item_data;
        bool isConsumable = itemData is ConsumableData;
        if (isConsumable && this._runContext.consumables_manager.is_full())
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && this._runContext.economy.gold >= this._price && this._hasEnoughHealth)
        {
            GetNode<AudioManager>("/root/AudioManager").play_button_click();
            this.item_purchased?.Invoke(this._item, this);
        }
    }

    private void _on_mouse_entered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        this.shop_slot_icon?.increased_icon_size();
    }

    private void _on_mouse_exited()
    {
        this.shop_slot_icon?.icon_normal_size();
    }

    private void _on_relic_added(string id)
    {
        if (id != "salmon_nigiri" && id != "butterfish_nigiri" && id != "soya_sauce" && id != "tuna_nigiri")
        {
            return;
        }

        if (this._item == null)
        {
            return;
        }

        Resource itemData = this._item.item_data;
        if (itemData is RelicData)
        {
            this.set_item(this._runContext.offers_manager.create_relic_offer_from_data(itemData as RelicData));
            return;
        }

        if (itemData is ConsumableData)
        {
            this.set_item(this._runContext.offers_manager.create_consumable_offer_from_data(itemData as ConsumableData));
        }
    }

    private void _on_status_health_change(int current_health)
    {
        this._chek_health(current_health, this._currentHealthCost);
    }

    private void _chek_health(int current_health, int health_cost)
    {
        this._hasEnoughHealth = current_health > health_cost;
    }
}
