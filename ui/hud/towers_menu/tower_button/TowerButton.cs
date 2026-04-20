using Godot;
using System;

public partial class TowerButton : Control
{
    public event Action<TowerDataWithInstance, int> TowerButtonPressed;
    public event Action<TowerButton> Hover;
    public event Action<TowerButton> Unhover;

    private static readonly StyleBox NORMAL_PANEL = GD.Load<StyleBox>("uid://dcjn1y7ofuii7");
    private static readonly StyleBox HOVER_PANEL = GD.Load<StyleBox>("uid://5m3jkdualcb3");

    private TowerDataWithInstance _towerData;
    private int _price = 0;
    private Texture2D _icon;
    private Texture2D _iconHover;
    private int _amount = 0;

    [Export]
    public TowerDataWithInstance TowerData
    {
        get => _towerData;
        set
        {
            _towerData = value;
            Configuration = value?.Data;
            TowerScene = value?.Scene;
            Icon = Configuration?.Icon;
            TowerType = Configuration?.type ?? 0;
            UpdatePrice();
        }
    }

    [Export]
    public NodePath PanelPath;

    [Export]
    public NodePath TowerButtonPath;

    [Export]
    public NodePath GoldPricePath;

    [Export]
    public NodePath AmountLabelPath;

    public TowerData Configuration;

    public int Price
    {
        get => _price;
        set
        {
            _price = value;
            if (_goldPriceNode != null)
            {
                _goldPriceNode.Price = value;
            }
        }
    }

    public Texture2D Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            UpdateTexture();
        }
    }

    public Texture2D IconHover
    {
        get => _iconHover;
        set
        {
            _iconHover = value;
            UpdateTextureHover();
        }
    }

    public int Amount
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

    public PackedScene TowerScene;
    public int TowerType;

    private Panel _panelNode;
    private TextureButton _towerButtonNode;
    private GoldPrice _goldPriceNode;
    private Label _amountLabelNode;
    private RunContext _runContext;

    public override void _Ready()
    {
        _panelNode = !PanelPath.IsEmpty ? GetNodeOrNull<Panel>(PanelPath) : GetNodeOrNull<Panel>("VBoxContainer/CenterContainer/Panel");
        _towerButtonNode = !TowerButtonPath.IsEmpty ? GetNodeOrNull<TextureButton>(TowerButtonPath) : GetNodeOrNull<TextureButton>("VBoxContainer/CenterContainer/TowerButton");
        _goldPriceNode = !GoldPricePath.IsEmpty ? GetNodeOrNull<GoldPrice>(GoldPricePath) : GetNodeOrNull<GoldPrice>("VBoxContainer/GoldPrice");
        _amountLabelNode = !AmountLabelPath.IsEmpty ? GetNodeOrNull<Label>(AmountLabelPath) : GetNodeOrNull<Label>("HBoxContainer/MarginContainer/AmountLabel");

        _runContext = GetNode<RunContext>("/root/RunContext");
        _runContext.Economy.AvailableFreeTowersChanged += OnAvailableFreeTowersChange;
        if (_runContext.RelicsManager != null)
        {
            _runContext.RelicsManager.RelicAdded += OnRelicAdded;
            _runContext.RelicsManager.RelicRemoved += OnRelicRemoved;
        }
        _runContext.Progress.CurrentWaveFinished += CurrentWaveFinished;

        _panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
        UpdateTexture();
    }

    public override void _ExitTree()
    {
        if (_runContext?.Economy != null)
        {
            _runContext.Economy.AvailableFreeTowersChanged -= OnAvailableFreeTowersChange;
        }

        if (_runContext?.Progress != null)
        {
            _runContext.Progress.CurrentWaveFinished -= CurrentWaveFinished;
        }

        if (_runContext?.RelicsManager != null)
        {
            _runContext.RelicsManager.RelicAdded -= OnRelicAdded;
            _runContext.RelicsManager.RelicRemoved -= OnRelicRemoved;
        }
    }

    private void UpdateTexture()
    {
        if (_towerButtonNode != null)
        {
            _towerButtonNode.TextureNormal = Icon;
        }
    }

    private void UpdateTextureHover()
    {
        if (_towerButtonNode != null)
        {
            _towerButtonNode.TextureHover = IconHover;
        }
    }

    private void OnMouseExited()
    {
        Unhover?.Invoke(this);
        _panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
    }

    private void OnMouseEntered()
    {
        _panelNode?.AddThemeStyleboxOverride("panel", HOVER_PANEL);
        Hover?.Invoke(this);
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

        if (runContext.Economy.AvailableFreeTowers > 0)
        {
            Price = 0;
            return;
        }

        if (Configuration == null)
        {
            return;
        }

        int basePrice = Configuration.BuildPrice;
        PriceContext ctx = new(PriceContext.PriceType.Tower, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);
        Price = ctx.FinalPrice;
    }

    private void OnAvailableFreeTowersChange(int availableFreeTowers)
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

    private void OnRelicRemoved(string relicId)
    {
        if (relicId == "soya_sauce" || relicId == "tuna_nigiri")
        {
            UpdatePrice();
        }
    }

    private void CurrentWaveFinished()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext?.RelicsManager != null && runContext.RelicsManager.HasRelic("lemon"))
        {
            UpdatePrice();
        }
    }

    private void OnTowerButtonPressed()
    {
        AudioManager audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_button_click();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext != null && runContext.Economy.Gold < Price)
        {
            return;
        }

        TowerButtonPressed?.Invoke(TowerData, Price);
    }
}

