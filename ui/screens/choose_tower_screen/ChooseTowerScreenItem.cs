using Godot;
using System;

public partial class ChooseTowerScreenItem : Control
{
    public event Action<ChooseTowerScreenItem> selected;

    [Export]
    public Label TitleLabel;

    [Export]
    public Label DescriptionLabel;

    [Export]
    public TextureRect texture;

    [Export]
    public TowerStatUi DamageStat;

    [Export]
    public TowerStatUi RangeStat;

    [Export]
    public TowerStatUi AttackSpeedStat;

    [Export]
    public GoldPrice GoldPrice;

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

        TitleLabel.Text = configuration.DisplayName;
        DescriptionLabel.Text = configuration.Description;
        texture.Texture = configuration.Icon;

        DamageStat.SetValue(configuration.BaseDamage);
        RangeStat.SetValue(configuration.BaseAttackRange);
        AttackSpeedStat.SetValue(configuration.BaseAttackSpeed);
        GoldPrice.price = configuration.BuildPrice;
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
