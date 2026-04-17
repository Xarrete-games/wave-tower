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
        ApplyConfiguration();
    }

    public void SetTowerData(TowerDataWithInstance towerData)
    {
        _towerData = towerData;
        if (IsNodeReady())
        {
            ApplyConfiguration();
        }
        else
        {
            CallDeferred(MethodName.ApplyConfigurationDeferred);
        }
    }

    public TowerDataWithInstance GetTowerData()
    {
        return _towerData;
    }

    private void ApplyConfiguration()
    {
        if (_towerData?.data == null)
        {
            return;
        }

        TowerData configuration = _towerData.data;

        title_label.Text = configuration.display_name;
        description_label.Text = configuration.description;
        texture.Texture = configuration.icon;

        damage_stat.SetValue(configuration.base_damage);
        range_stat.SetValue(configuration.base_attack_range);
        attack_speed_stat.SetValue(configuration.base_attack_speed);
        gold_price.price = configuration.build_price;
    }

    private void ApplyConfigurationDeferred()
    {
        ApplyConfiguration();
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (!InputClickUtils.IsLeftClickReleased(@event))
        {
            return;
        }

        GetNode<AudioManager>("/root/AudioManager").play_button_click();
        selected?.Invoke(this);
    }
}