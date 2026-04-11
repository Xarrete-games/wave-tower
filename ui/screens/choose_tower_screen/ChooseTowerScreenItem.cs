using Godot;

public partial class ChooseTowerScreenItem : Control
{
    [Signal]
    public delegate void selectedEventHandler(ChooseTowerScreenItem item);

    [Export]
    public Label title_label;

    [Export]
    public Label description_label;

    [Export]
    public TextureRect texture;

    [Export]
    public Node damage_stat;

    [Export]
    public Node range_stat;

    [Export]
    public Node attack_speed_stat;

    [Export]
    public Node gold_price;

    private Variant _towerData = default;

    public override void _Ready()
    {
        this.ApplyConfiguration();
    }

    public void SetTowerData(Variant towerData)
    {
        this._towerData = towerData;
        if (this.IsNodeReady())
        {
            this.ApplyConfiguration();
        }
        else
        {
            this.CallDeferred(MethodName.ApplyConfigurationDeferred);
        }
    }

    public Variant GetTowerData()
    {
        return this._towerData;
    }

    private void ApplyConfiguration()
    {
        if (this._towerData.VariantType == Variant.Type.Nil)
        {
            return;
        }

        GodotObject towerDataObj = this._towerData.AsGodotObject();
        GodotObject configuration = towerDataObj?.Get("data").AsGodotObject();
        if (configuration == null)
        {
            return;
        }

        this.title_label.Text = (string)configuration.Get("display_name");
        this.description_label.Text = (string)configuration.Get("description");
        this.texture.Texture = configuration.Get("icon").As<Texture2D>();

        this.damage_stat.Call("set_value", configuration.Get("base_damage"));
        this.range_stat.Call("set_value", configuration.Get("base_attack_range"));
        this.attack_speed_stat.Call("set_value", configuration.Get("base_attack_speed"));
        this.gold_price.Set("price", configuration.Get("build_price"));
    }

    private void ApplyConfigurationDeferred()
    {
        this.ApplyConfiguration();
    }

    private void _on_gui_input(InputEvent @event)
    {
        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        GetNode<Node>("/root/AudioManager").Call("play_button_click");
        EmitSignal(SignalName.selected, this);
    }
}