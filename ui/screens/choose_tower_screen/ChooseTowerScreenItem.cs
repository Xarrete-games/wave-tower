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
        if (_towerData?.Data == null)
        {
            return;
        }

        TowerData configuration = _towerData.Data;

        title_label.Text = configuration.DisplayName;
        description_label.Text = configuration.Description;
        texture.Texture = configuration.Icon;

        damage_stat.SetValue(configuration.BaseDamage);
        range_stat.SetValue(configuration.BaseAttackRange);
        attack_speed_stat.SetValue(configuration.BaseAttackSpeed);
        gold_price.price = configuration.BuildPrice;
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
