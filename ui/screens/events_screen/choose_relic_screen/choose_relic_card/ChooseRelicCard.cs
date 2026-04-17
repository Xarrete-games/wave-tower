using Godot;
using System;

public partial class ChooseRelicCard : Control
{
    public event Action<RelicData> card_pressed;

    private RelicData _relicData;
    private bool _hasEnoughLife;
    private bool _itCostsHealth;
    private int _healthCost;
    private RunContext _runContext;
    private bool _isHealthSubscribed;

    private RichTextLabel _description;
    private Label _title;
    private TextureRect _relicTexture;
    private Polygon2D _hexagonBorder;
    private HealthPrice _healthPrice;

    public override void _Ready()
    {
        _description = GetNode<RichTextLabel>("VBoxContainer/Description");
        _title = GetNode<Label>("VBoxContainer/Title");
        _relicTexture = GetNode<TextureRect>("RelicIcon/RelicTexture");
        _hexagonBorder = GetNode<Polygon2D>("RelicIcon/Hexagon/Control/Root2d/HexagonBorder");
        _healthPrice = GetNode<HealthPrice>("VBoxContainer/HealthPrice");
        _runContext = GetNode<RunContext>("/root/RunContext");
        _healthPrice.Visible = false;
    }

    public override void _ExitTree()
    {
        if (_runContext?.status != null && _isHealthSubscribed)
        {
            _runContext.status.health_change -= OnHealthChanged;
            _isHealthSubscribed = false;
        }
    }

    public void set_relic(RelicData newRelicData)
    {
        _relicData = newRelicData;
        RelicData data = newRelicData;
        if (data == null)
        {
            return;
        }

        _relicTexture.Texture = data.icon;
        _title.Text = data.DisplayName;
        _description.Text = data.description;

        _healthCost = data.HealthPrice;
        if (_healthCost > 0)
        {
            _healthPrice.Visible = true;
            _itCostsHealth = true;
            _healthPrice.price = _healthCost;

            CheckHealth(_runContext.status.health, _healthCost);
            if (!_isHealthSubscribed)
            {
                _runContext.status.health_change += OnHealthChanged;
                _isHealthSubscribed = true;
            }
        }
        else
        {
            _healthPrice.Visible = false;
            _itCostsHealth = false;
            _hasEnoughLife = true;

            if (_runContext?.status != null && _isHealthSubscribed)
            {
                _runContext.status.health_change -= OnHealthChanged;
                _isHealthSubscribed = false;
            }
        }

        _hexagonBorder.Color = _runContext.relics_manager.get_rarity_color(data.rarity);
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (_itCostsHealth && !_hasEnoughLife)
        {
            return;
        }

        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        card_pressed?.Invoke(_relicData);
    }

    private void CheckHealth(int currentHealth, int healthCost)
    {
        _hasEnoughLife = currentHealth > healthCost;
    }

    private void OnHealthChanged(int currentHealth)
    {
        CheckHealth(currentHealth, _healthCost);
    }

    private void OnMouseEntered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        _relicTexture.CustomMinimumSize = new Vector2(130, 130);
    }

    private void OnMouseExited()
    {
        _relicTexture.CustomMinimumSize = new Vector2(80, 80);
    }
}
