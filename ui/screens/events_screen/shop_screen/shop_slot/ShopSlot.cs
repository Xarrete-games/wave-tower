using Godot;
using System;

public partial class ShopSlot : VBoxContainer
{
    public event Action<ItemOffer, ShopSlot> ItemPurchased;

    [Export]
    public Label TitleLabel;

    [Export]
    public RichTextLabel DescriptionLabel;

    [Export]
    public GoldPrice GoldPrice;

    [Export]
    public ShopSlotIcon ShopSlotIcon;

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

    public void SetItem(ItemOffer itemOffer)
    {
        if (itemOffer == null)
        {
            return;
        }

        _item = itemOffer;
        _price = itemOffer.Price;

        Resource itemData = itemOffer.ItemData;
        if (itemData == null)
        {
            return;
        }

        string displayName = string.Empty;
        string description = string.Empty;
        Texture2D icon = null;
        if (itemData is RelicData relicInfo)
        {
            displayName = relicInfo.DisplayName;
            description = relicInfo.Description;
            icon = relicInfo.Icon;
        }
        else if (itemData is ConsumableData consumableInfo)
        {
            displayName = consumableInfo.DisplayName;
            description = consumableInfo.Description;
            icon = consumableInfo.Icon;
        }

        TitleLabel.Text = displayName;
        DescriptionLabel.Text = description;
        TooltipText = description;
        if (GoldPrice != null)
        {
            GoldPrice.price = _price;
        }
        ShopSlotIcon?.set_icon(icon);

        RelicData relicData = itemData as RelicData;
        if (relicData != null)
        {
            ShopSlotIcon?.set_background_color(_runContext.relics_manager.get_rarity_color(relicData.Rarity));
        }

        _currentHealthCost = itemOffer.HealthPrice;
        _healthPrice.Visible = _currentHealthCost > 0;
        if (_currentHealthCost > 0)
        {
            _healthPrice.price = _currentHealthCost;
        }

        CheckHealth(_runContext.status.health, _currentHealthCost);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (_item == null)
        {
            return;
        }

        Resource itemData = _item.ItemData;
        bool isConsumable = itemData is ConsumableData;
        if (isConsumable && _runContext.consumables_manager.is_full())
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && _runContext.economy.gold >= _price && _hasEnoughHealth)
        {
            GetNode<AudioManager>("/root/AudioManager").play_button_click();
            ItemPurchased?.Invoke(_item, this);
        }
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        ShopSlotIcon?.increased_icon_size();
    }

    private void OnMouseExited()
    {
        ShopSlotIcon?.icon_normal_size();
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

        Resource itemData = _item.ItemData;
        if (itemData is RelicData)
        {
            SetItem(_runContext.offers_manager.create_relic_offer_from_data(itemData as RelicData));
            return;
        }

        if (itemData is ConsumableData)
        {
            SetItem(_runContext.offers_manager.create_consumable_offer_from_data(itemData as ConsumableData));
        }
    }

    private void OnStatusHealthChange(int currentHealth)
    {
        CheckHealth(currentHealth, _currentHealthCost);
    }

    private void CheckHealth(int currentHealth, int healthCost)
    {
        _hasEnoughHealth = currentHealth > healthCost;
    }
}
