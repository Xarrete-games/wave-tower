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
    public GoldPrice gold_price;

    [Export]
    public ShopSlotIcon shop_slot_icon;

    private HealthPrice _healthPrice;
    private RunContext _runContext;
    private ItemOffer _item;
    private int _price;
    private bool _hasEnoughHealth;
    private int _currentHealthCost;

    public override void _Ready()
    {
        _healthPrice = GetNode<HealthPrice>("HealthPrice");
        _healthPrice.Visible = false;

        _runContext = GetNode<RunContext>("/root/RunContext");
        if (_runContext?.relics_manager != null)
        {
            _runContext.relics_manager.relic_added += OnRelicAdded;
        }

        if (_runContext?.status != null)
        {
            _runContext.status.health_change += OnStatusHealthChange;
        }
    }

    public override void _ExitTree()
    {
        if (_runContext?.relics_manager != null)
        {
            _runContext.relics_manager.relic_added -= OnRelicAdded;
        }

        if (_runContext?.status != null)
        {
            _runContext.status.health_change -= OnStatusHealthChange;
        }
    }

    public void set_item(ItemOffer itemOffer)
    {
        if (itemOffer == null)
        {
            return;
        }

        _item = itemOffer;
        _price = itemOffer.price;

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

        title_label.Text = displayName;
        description_label.Text = description;
        TooltipText = description;
        if (gold_price != null)
        {
            gold_price.price = _price;
        }
        shop_slot_icon?.set_icon(icon);

        RelicData relicData = itemData as RelicData;
        if (relicData != null)
        {
            shop_slot_icon?.set_background_color(_runContext.relics_manager.get_rarity_color(relicData.rarity));
        }

        _currentHealthCost = itemOffer.health_price;
        _healthPrice.Visible = _currentHealthCost > 0;
        if (_currentHealthCost > 0)
        {
            _healthPrice.price = _currentHealthCost;
        }

        ChekHealth(_runContext.status.health, _currentHealthCost);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (_item == null)
        {
            return;
        }

        Resource itemData = _item.item_data;
        bool isConsumable = itemData is ConsumableData;
        if (isConsumable && _runContext.consumables_manager.is_full())
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && _runContext.economy.gold >= _price && _hasEnoughHealth)
        {
            GetNode<AudioManager>("/root/AudioManager").play_button_click();
            item_purchased?.Invoke(_item, this);
        }
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        shop_slot_icon?.increased_icon_size();
    }

    private void OnMouseExited()
    {
        shop_slot_icon?.icon_normal_size();
    }

    private void OnRelicAdded(string id)
    {
        if (id != "salmon_nigiri" && id != "butterfish_nigiri" && id != "soya_sauce" && id != "tuna_nigiri")
        {
            return;
        }

        if (_item == null)
        {
            return;
        }

        Resource itemData = _item.item_data;
        if (itemData is RelicData)
        {
            set_item(_runContext.offers_manager.create_relic_offer_from_data(itemData as RelicData));
            return;
        }

        if (itemData is ConsumableData)
        {
            set_item(_runContext.offers_manager.create_consumable_offer_from_data(itemData as ConsumableData));
        }
    }

    private void OnStatusHealthChange(int current_health)
    {
        ChekHealth(current_health, _currentHealthCost);
    }

    private void ChekHealth(int current_health, int health_cost)
    {
        _hasEnoughHealth = current_health > health_cost;
    }
}
