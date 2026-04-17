using Godot;
using System;

public partial class TowerButton : Control
{
    public event Action<TowerDataWithInstance, int> tower_button_pressed;
    public event Action<TowerButton> hover;
    public event Action<TowerButton> unhover;

    private static readonly StyleBox NORMAL_PANEL = GD.Load<StyleBox>("uid://dcjn1y7ofuii7");
    private static readonly StyleBox HOVER_PANEL = GD.Load<StyleBox>("uid://5m3jkdualcb3");

    private TowerDataWithInstance _tower_data;
    private int _price = 0;
    private Texture2D _icon;
    private Texture2D _icon_hover;
    private int _amount = 0;

    [Export]
    public TowerDataWithInstance tower_data
    {
        get => _tower_data;
        set
        {
            _tower_data = value;
            configuration = value?.data;
            tower_scene = value?.scene;
            icon = configuration?.icon;
            type = configuration?.type ?? 0;
            UpdatePrice();
        }
    }

    [Export]
    public NodePath panel;

    [Export]
    public NodePath tower_button;

    [Export]
    public NodePath gold_price;

    [Export]
    public NodePath amount_label;

    public TowerData configuration;

    public int price
    {
        get => _price;
        set
        {
            _price = value;
            if (_goldPriceNode != null)
            {
                _goldPriceNode.price = value;
            }
        }
    }

    public Texture2D icon
    {
        get => _icon;
        set
        {
            _icon = value;
            UpdateTexture();
        }
    }

    public Texture2D icon_hover
    {
        get => _icon_hover;
        set
        {
            _icon_hover = value;
            UpdateTextureHover();
        }
    }

    public int amount
    {
        get => _amount;
        set
        {
            _amount = value;
            if (_amountLabelNode != null)
            {
                _amountLabelNode.Text = "x " + value;
            }
        }
    }

    public PackedScene tower_scene;
    public int type;

    private Panel _panelNode;
    private TextureButton _towerButtonNode;
    private GoldPrice _goldPriceNode;
    private Label _amountLabelNode;
    private RunContext _runContext;

    public override void _Ready()
    {
        _panelNode = !panel.IsEmpty ? GetNodeOrNull<Panel>(panel) : GetNodeOrNull<Panel>("VBoxContainer/CenterContainer/Panel");
        _towerButtonNode = !tower_button.IsEmpty ? GetNodeOrNull<TextureButton>(tower_button) : GetNodeOrNull<TextureButton>("VBoxContainer/CenterContainer/TowerButton");
        _goldPriceNode = !gold_price.IsEmpty ? GetNodeOrNull<GoldPrice>(gold_price) : GetNodeOrNull<GoldPrice>("VBoxContainer/GoldPrice");
        _amountLabelNode = !amount_label.IsEmpty ? GetNodeOrNull<Label>(amount_label) : GetNodeOrNull<Label>("HBoxContainer/MarginContainer/AmountLabel");

        _runContext = GetNode<RunContext>("/root/RunContext");
        _runContext.economy.available_free_towers_change += OnAvailableFreeTowersChange;
        if (_runContext.relics_manager != null)
        {
            _runContext.relics_manager.relic_added += OnRelicAdded;
            _runContext.relics_manager.relic_removed += OnRelicRemoved;
        }
        _runContext.progress.current_wave_finished += CurrentWaveFinished;

        _panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
        UpdateTexture();
    }

    public override void _ExitTree()
    {
        if (_runContext?.economy != null)
        {
            _runContext.economy.available_free_towers_change -= OnAvailableFreeTowersChange;
        }

        if (_runContext?.progress != null)
        {
            _runContext.progress.current_wave_finished -= CurrentWaveFinished;
        }

        if (_runContext?.relics_manager != null)
        {
            _runContext.relics_manager.relic_added -= OnRelicAdded;
            _runContext.relics_manager.relic_removed -= OnRelicRemoved;
        }
    }

    private void UpdateTexture()
    {
        if (_towerButtonNode != null)
        {
            _towerButtonNode.TextureNormal = icon;
        }
    }

    private void UpdateTextureHover()
    {
        if (_towerButtonNode != null)
        {
            _towerButtonNode.TextureHover = icon_hover;
        }
    }

    private void OnMouseExited()
    {
        unhover?.Invoke(this);
        _panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
    }

    private void OnMouseEntered()
    {
        _panelNode?.AddThemeStyleboxOverride("panel", HOVER_PANEL);
        hover?.Invoke(this);
        AudioManager audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_button_hover();
    }

    private void UpdatePrice()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext == null)
        {
            return;
        }

        if (runContext.economy.available_free_towers > 0)
        {
            price = 0;
            return;
        }

        if (configuration == null)
        {
            return;
        }

        int basePrice = configuration.build_price;
        PriceContext ctx = new(PriceContext.PriceType.Tower, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);
        price = ctx.FinalPrice;
    }

    private void OnAvailableFreeTowersChange(int _available_free_towers)
    {
        UpdatePrice();
    }

    private void OnRelicAdded(string relicId)
    {
        if (relicId == "soya_sauce" || relicId == "tuna_nigiri")
        {
            UpdatePrice();
        }
    }

    private void OnRelicRemoved(string relic_id)
    {
        if (relic_id == "soya_sauce" || relic_id == "tuna_nigiri")
        {
            UpdatePrice();
        }
    }

    private void CurrentWaveFinished()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext?.relics_manager != null && runContext.relics_manager.has_relic("lemon"))
        {
            UpdatePrice();
        }
    }

    private void OnTowerButtonPressed()
    {
        AudioManager audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_button_click();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext != null && runContext.economy.gold < price)
        {
            return;
        }

        tower_button_pressed?.Invoke(tower_data, price);
    }
}
