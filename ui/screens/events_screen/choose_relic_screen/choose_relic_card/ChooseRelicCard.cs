using Godot;
using System;

public partial class ChooseRelicCard : Control
{
    public event Action<RelicData> CardPressed;

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
        if (_runContext?.Status != null && _isHealthSubscribed)
        {
            _runContext.Status.HealthChanged -= OnHealthChanged;
            _isHealthSubscribed = false;
        }
    }

    public void SetRelic(RelicData newRelicData)
    {
        _relicData = newRelicData;
        RelicData data = newRelicData;
        if (data == null)
        {
            return;
        }

        _relicTexture.Texture = data.Icon;
        _title.Text = data.DisplayName;
        _description.Text = data.Description;

        _healthCost = data.HealthPrice;
        if (_healthCost > 0)
        {
            _healthPrice.Visible = true;
            _itCostsHealth = true;
            _healthPrice.price = _healthCost;

            CheckHealth(_runContext.Status.Health, _healthCost);
            if (!_isHealthSubscribed)
            {
                _runContext.Status.HealthChanged += OnHealthChanged;
                _isHealthSubscribed = true;
            }
        }
        else
        {
            _healthPrice.Visible = false;
            _itCostsHealth = false;
            _hasEnoughLife = true;

            if (_runContext?.Status != null && _isHealthSubscribed)
            {
                _runContext.Status.HealthChanged -= OnHealthChanged;
                _isHealthSubscribed = false;
            }
        }

        _hexagonBorder.Color = _runContext.RelicsManager.GetRarityColor(data.Rarity);
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
        CardPressed?.Invoke(_relicData);
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
