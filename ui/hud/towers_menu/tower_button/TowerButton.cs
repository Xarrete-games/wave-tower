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
        get => this._tower_data;
        set
        {
            this._tower_data = value;
            this.configuration = value?.data;
            this.tower_scene = value?.scene;
            this.icon = this.configuration?.icon;
            this.type = this.configuration?.type ?? 0;
            this._update_price();
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
        get => this._price;
        set
        {
            this._price = value;
                if (this._goldPriceNode != null)
                {
                    this._goldPriceNode.price = value;
                }
        }
    }

    public Texture2D icon
    {
        get => this._icon;
        set
        {
            this._icon = value;
            this._update_texture();
        }
    }

    public Texture2D icon_hover
    {
        get => this._icon_hover;
        set
        {
            this._icon_hover = value;
            this._update_texture_hover();
        }
    }

    public int amount
    {
        get => this._amount;
        set
        {
            this._amount = value;
            if (this._amountLabelNode != null)
            {
                this._amountLabelNode.Text = "x " + value;
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
        this._panelNode = !this.panel.IsEmpty ? GetNodeOrNull<Panel>(this.panel) : GetNodeOrNull<Panel>("VBoxContainer/CenterContainer/Panel");
        this._towerButtonNode = !this.tower_button.IsEmpty ? GetNodeOrNull<TextureButton>(this.tower_button) : GetNodeOrNull<TextureButton>("VBoxContainer/CenterContainer/TowerButton");
        this._goldPriceNode = !this.gold_price.IsEmpty ? GetNodeOrNull<GoldPrice>(this.gold_price) : GetNodeOrNull<GoldPrice>("VBoxContainer/GoldPrice");
        this._amountLabelNode = !this.amount_label.IsEmpty ? GetNodeOrNull<Label>(this.amount_label) : GetNodeOrNull<Label>("HBoxContainer/MarginContainer/AmountLabel");

        this._runContext = GetNode<RunContext>("/root/RunContext");
        this._runContext.economy.available_free_towers_change += this._on_available_free_towers_change;
        if (this._runContext.relics_manager != null)
        {
            this._runContext.relics_manager.relic_added += this._on_relic_added;
            this._runContext.relics_manager.relic_removed += this._on_relic_removed;
        }
        this._runContext.progress.current_wave_finished += this._current_wave_finished;

        this._panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
        this._update_texture();
    }

    public override void _ExitTree()
    {
        if (this._runContext?.economy != null)
        {
            this._runContext.economy.available_free_towers_change -= this._on_available_free_towers_change;
        }

        if (this._runContext?.progress != null)
        {
            this._runContext.progress.current_wave_finished -= this._current_wave_finished;
        }

        if (this._runContext?.relics_manager != null)
        {
            this._runContext.relics_manager.relic_added -= this._on_relic_added;
            this._runContext.relics_manager.relic_removed -= this._on_relic_removed;
        }
    }

    private void _update_texture()
    {
        if (this._towerButtonNode != null)
        {
            this._towerButtonNode.TextureNormal = this.icon;
        }
    }

    private void _update_texture_hover()
    {
        if (this._towerButtonNode != null)
        {
            this._towerButtonNode.TextureHover = this.icon_hover;
        }
    }

    private void _on_mouse_exited()
    {
        this.unhover?.Invoke(this);
        this._panelNode?.AddThemeStyleboxOverride("panel", NORMAL_PANEL);
    }

    private void _on_mouse_entered()
    {
        this._panelNode?.AddThemeStyleboxOverride("panel", HOVER_PANEL);
        this.hover?.Invoke(this);
        AudioManager audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_button_hover();
    }

    private void _update_price()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext == null)
        {
            return;
        }

        if (runContext.economy.available_free_towers > 0)
        {
            this.price = 0;
            return;
        }

        if (this.configuration == null)
        {
            return;
        }

        int basePrice = this.configuration.build_price;
        PriceContext ctx = new(PriceContext.PriceType.Tower, basePrice);
        Hooks.OnGetPrice(Hooks.GetListenersFromRuntime(), ctx);
        this.price = ctx.FinalPrice;
    }

    private void _on_available_free_towers_change(int _available_free_towers)
    {
        this._update_price();
    }

    private void _on_relic_added(string relicId)
    {
        if (relicId == "soya_sauce" || relicId == "tuna_nigiri")
        {
            this._update_price();
        }
    }

    private void _on_relic_removed(string relic_id)
    {
        if (relic_id == "soya_sauce" || relic_id == "tuna_nigiri")
        {
            this._update_price();
        }
    }

    private void _current_wave_finished()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext?.relics_manager != null && runContext.relics_manager.has_relic("lemon"))
        {
            this._update_price();
        }
    }

    private void _on_tower_button_pressed()
    {
        AudioManager audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
        audioManager?.play_button_click();

        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext != null && runContext.economy.gold < this.price)
        {
            return;
        }

        this.tower_button_pressed?.Invoke(this.tower_data, this.price);
    }
}
