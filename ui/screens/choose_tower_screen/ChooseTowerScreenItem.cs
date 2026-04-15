using Godot;
using System;

public partial class ChooseTowerScreenItem : Control
{
    public event Action<ChooseTowerScreenItem> selected;

    [Export]
    public Label title_label;

    [Export]
    public Label description_label;

    [Export]
    public TextureRect texture;

    [Export]
    public TowerStatUi damage_stat;

    [Export]
    public TowerStatUi range_stat;

    [Export]
    public TowerStatUi attack_speed_stat;

    [Export]
    public GoldPrice gold_price;

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

        this.damage_stat.set_value(configuration.base_damage);
        this.range_stat.set_value(configuration.base_attack_range);
        this.attack_speed_stat.set_value(configuration.base_attack_speed);
        this.gold_price.price = configuration.build_price;
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

        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        this.selected?.Invoke(this);
    }
}