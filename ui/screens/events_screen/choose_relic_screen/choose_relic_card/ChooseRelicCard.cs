using Godot;
using System;

public partial class ChooseRelicCard : Control
{
    public event Action<Variant> card_pressed;

    private Variant _relicData;
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
        this._description = GetNode<RichTextLabel>("VBoxContainer/Description");
        this._title = GetNode<Label>("VBoxContainer/Title");
        this._relicTexture = GetNode<TextureRect>("RelicIcon/RelicTexture");
        this._hexagonBorder = GetNode<Polygon2D>("RelicIcon/Hexagon/Control/Root2d/HexagonBorder");
        this._healthPrice = GetNode<HealthPrice>("VBoxContainer/HealthPrice");
        this._runContext = GetNode<RunContext>("/root/RunContext");
        this._healthPrice.Visible = false;
    }

    public override void _ExitTree()
    {
        if (this._runContext?.status != null && this._isHealthSubscribed)
        {
            this._runContext.status.health_change -= this.OnHealthChanged;
            this._isHealthSubscribed = false;
        }
    }

    public void set_relic(Variant newRelicData)
    {
        this._relicData = newRelicData;
        RelicData data = newRelicData.As<RelicData>();
        if (data == null)
        {
            return;
        }

        this._relicTexture.Texture = data.icon;
        this._title.Text = data.display_name;
        this._description.Text = data.description;

        this._healthCost = data.health_price;
        if (this._healthCost > 0)
        {
            this._healthPrice.Visible = true;
            this._itCostsHealth = true;
            this._healthPrice.price = this._healthCost;

            this.CheckHealth(this._runContext.status.health, this._healthCost);
            if (!this._isHealthSubscribed)
            {
                this._runContext.status.health_change += this.OnHealthChanged;
                this._isHealthSubscribed = true;
            }
        }
        else
        {
            this._healthPrice.Visible = false;
            this._itCostsHealth = false;
            this._hasEnoughLife = true;

            if (this._runContext?.status != null && this._isHealthSubscribed)
            {
                this._runContext.status.health_change -= this.OnHealthChanged;
                this._isHealthSubscribed = false;
            }
        }

        this._hexagonBorder.Color = this._runContext.relics_manager.get_rarity_color(data.rarity);
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (this._itCostsHealth && !this._hasEnoughLife)
        {
            return;
        }

        if (!UIUtilsStatic.IsLeftClickEvent(@event))
        {
            return;
        }

        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        this.card_pressed?.Invoke(this._relicData);
    }

    private void CheckHealth(int currentHealth, int healthCost)
    {
        this._hasEnoughLife = currentHealth > healthCost;
    }

    private void OnHealthChanged(int currentHealth)
    {
        this.CheckHealth(currentHealth, this._healthCost);
    }

    private void _on_mouse_entered()
    {
        GetNode<AudioManager>("/root/AudioManager").play_button_hover();
        this._relicTexture.CustomMinimumSize = new Vector2(130, 130);
    }

    private void _on_mouse_exited()
    {
        this._relicTexture.CustomMinimumSize = new Vector2(80, 80);
    }
}
