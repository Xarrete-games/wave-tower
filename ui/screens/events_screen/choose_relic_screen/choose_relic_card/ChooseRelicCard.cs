using Godot;

public partial class ChooseRelicCard : Control
{
    [Signal]
    public delegate void card_pressedEventHandler(Variant relicData);

    private Variant _relicData;
    private bool _hasEnoughLife;
    private bool _itCostsHealth;

    private RichTextLabel _description;
    private Label _title;
    private TextureRect _relicTexture;
    private Polygon2D _hexagonBorder;
    private Control _healthPrice;

    public override void _Ready()
    {
        this._description = GetNode<RichTextLabel>("VBoxContainer/Description");
        this._title = GetNode<Label>("VBoxContainer/Title");
        this._relicTexture = GetNode<TextureRect>("RelicIcon/RelicTexture");
        this._hexagonBorder = GetNode<Polygon2D>("RelicIcon/Hexagon/Control/Root2d/HexagonBorder");
        this._healthPrice = GetNode<Control>("VBoxContainer/HealthPrice");
        this._healthPrice.Visible = false;
    }

    public void set_relic(Variant newRelicData)
    {
        this._relicData = newRelicData;
        GodotObject data = newRelicData.AsGodotObject();
        if (data == null)
        {
            return;
        }

        this._relicTexture.Texture = data.Get("icon").As<Texture2D>();
        this._title.Text = data.Get("display_name").AsString();
        this._description.Text = data.Get("description").AsString();

        int healthPrice = (int)data.Get("health_price");
        if (healthPrice > 0)
        {
            this._healthPrice.Visible = true;
            this._itCostsHealth = true;
            this._healthPrice.Set("price", healthPrice);

            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            this.CheckHealth((int)runContext.status.Get("health"), healthPrice);
            runContext.status.Connect("health_change", Callable.From<int>(currentHealth =>
                this.CheckHealth(currentHealth, healthPrice)));
        }

        RunContext context = GetNode<RunContext>("/root/RunContext");
        this._hexagonBorder.Color = context.relics_manager.get_rarity_color((int)data.Get("rarity"));
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

        GetNode<Node>("/root/AudioManager").Call("play_button_click");
        EmitSignal(SignalName.card_pressed, this._relicData);
    }

    private void CheckHealth(int currentHealth, int healthCost)
    {
        this._hasEnoughLife = currentHealth > healthCost;
    }

    private void _on_mouse_entered()
    {
        GetNode<Node>("/root/AudioManager").Call("play_button_hover");
        this._relicTexture.CustomMinimumSize = new Vector2(130, 130);
    }

    private void _on_mouse_exited()
    {
        this._relicTexture.CustomMinimumSize = new Vector2(80, 80);
    }
}
