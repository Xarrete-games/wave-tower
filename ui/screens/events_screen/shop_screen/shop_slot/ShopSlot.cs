using Godot;
using System;

public partial class ShopSlot : Control
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
        if (_runContext?.RelicsManager != null)
        {
            _runContext.RelicsManager.RelicAdded += OnRelicAdded;
        }

        if (_runContext?.Status != null)
        {
            _runContext.Status.HealthChanged += OnStatusHealthChange;
        }
    }

    public override void _ExitTree()
    {
        if (_runContext?.RelicsManager != null)
        {
            _runContext.RelicsManager.RelicAdded -= OnRelicAdded;
        }

        if (_runContext?.Status != null)
        {
            _runContext.Status.HealthChanged -= OnStatusHealthChange;
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
            GoldPrice.Price = _price;
        }
        ShopSlotIcon?.SetIcon(icon);

        RelicData relicData = itemData as RelicData;
        if (relicData != null)
        {
            ShopSlotIcon?.SetBackgroundColor(_runContext.RelicsManager.GetRarityColor(relicData.Rarity));
        }

        _currentHealthCost = itemOffer.HealthPrice;
        _healthPrice.Visible = _currentHealthCost > 0;
        if (_currentHealthCost > 0)
        {
            _healthPrice.Price = _currentHealthCost;
        }

        CheckHealth(_runContext.Status.Health, _currentHealthCost);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (_item == null)
        {
            return;
        }

        Resource itemData = _item.ItemData;
        bool isConsumable = itemData is ConsumableData;
        if (isConsumable && _runContext.ConsumablesManager.IsFull())
        {
            return;
        }

        if (UIUtilsStatic.IsLeftClickEvent(@event) && _runContext.Economy.Gold >= _price && _hasEnoughHealth)
        {
            GetNode<AudioManager>("/root/AudioManager").play_button_click();
            ItemPurchased?.Invoke(_item, this);
        }
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        ShopSlotIcon?.IncreaseIconSize();
    }

    private void OnMouseExited()
    {
        ShopSlotIcon?.SetIconNormalSize();
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
            SetItem(_runContext.OffersManager.CreateRelicOfferFromData(itemData as RelicData));
            return;
        }

        if (itemData is ConsumableData)
        {
            SetItem(_runContext.OffersManager.CreateConsumableOfferFromData(itemData as ConsumableData));
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
