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

    private TowerDataWithInstance _towerData;

    public override void _Ready()
    {
        this.ApplyConfiguration();
    }

    public void SetTowerData(TowerDataWithInstance towerData)
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

    public TowerDataWithInstance GetTowerData()
    {
        return this._towerData;
    }

    private void ApplyConfiguration()
    {
        if (this._towerData?.data == null)
        {
            return;
        }

        TowerData configuration = this._towerData.data;

        this.title_label.Text = configuration.display_name;
        this.description_label.Text = configuration.description;
        this.texture.Texture = configuration.icon;

        this.damage_stat.Call("set_value", configuration.base_damage);
        this.range_stat.Call("set_value", configuration.base_attack_range);
        this.attack_speed_stat.Call("set_value", configuration.base_attack_speed);
        this.gold_price.Set("price", configuration.build_price);
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