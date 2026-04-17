using Godot;
using System.Collections.Generic;

public partial class TowerStatsPanel : Control
{
    private Label _nameLabel;
    private Label _idLabel;
    private TowerStatUi _damageStat;
    private TowerStatUi _attackSpeedStat;
    private TowerStatUi _rangeStat;
    private Label _levelLabel;
    private Control _upgradeButtonContainer;
    private GoldPrice _upgradeTowerPrice;
    private OptionButton _targetingModeSelector;
    private Control _towerHintPanel;

    private Tower _currentTower;

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/NameLabel");
        _idLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/IdLabel");
        _damageStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/DamageStatUi");
        _attackSpeedStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/AttkSpeedStatUi");
        _rangeStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/RangeStatUi");
        _levelLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/ExpDataContainer/LevelContainer/LevelLabel");
        _upgradeButtonContainer = GetNode<Control>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer");
        _upgradeTowerPrice = GetNode<GoldPrice>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer/UpgradeTowerPrice");
        _targetingModeSelector = GetNode<OptionButton>("Panel/MarginContainer/MainContainer/TargetingContainer/TargetingModeSelector");
        _towerHintPanel = GetNode<Control>("TowerHintPanel");

        Visible = false;
        _towerHintPanel.Visible = false;
        HideUpgradeOptions();

        ClickEvents.TowerSelected += OnTowerSelected;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerSelected -= OnTowerSelected;
    }

    private void OnTowerSelected(Tower tower)
    {
        if (tower == null)
        {
            _currentTower = null;
            ActionManager actionManager = GetNode<ActionManager>("/root/ActionManager");
            if (actionManager.CurrentAction == ActionManager.ActionState.TowerSelected)
            {
                actionManager.EndAction();
            }

            return;
        }

        var targetingModes = new List<TowerTargetingMode>
        {
            TowerTargetingMode.FirstInProgress,
        };
        Hooks.OnGetTargetingModes(Hooks.GetListenersFromRuntime(), targetingModes);
        UpdateTargetingModes(targetingModes);

        int targetingMode = tower.targeting_mode;
        _targetingModeSelector.Select(targetingMode);

        ActionManager manager = GetNode<ActionManager>("/root/ActionManager");
        manager.StartAction(ActionManager.ActionState.TowerSelected, HidePanel);
        Visible = true;

        TowerStats stats = tower.stats;
        TowerExpData expData = tower.exp_data;
        UpdateStats(stats);
        UpdateExpData(expData);

        TowerData data = tower.data as TowerData;
        _nameLabel.Text = data?.display_name ?? string.Empty;
        _idLabel.Text = tower.id;
        _currentTower = tower;

        _levelLabel.Text = tower.level.ToString();
        if (tower.is_max_level())
        {
            HideUpgradeOptions();
        }
        else
        {
            _upgradeTowerPrice.price = data?.upgrade_price ?? 0;
            _upgradeButtonContainer.Visible = true;
        }
    }

    private void HidePanel()
    {
        Visible = false;
    }

    private void UpdateStats(TowerStats towerStats)
    {
        if (towerStats == null)
        {
            return;
        }

        _damageStat.set_value(towerStats.damage);
        _attackSpeedStat.set_value(towerStats.attack_speed);
        _rangeStat.set_value(towerStats.attack_range);
    }

    private void UpdateExpData(TowerExpData expData)
    {
        if (expData == null)
        {
            return;
        }

        _levelLabel.Text = expData.level.ToString();
    }

    private void UpdateTargetingModes(List<TowerTargetingMode> modes)
    {
        _targetingModeSelector.Clear();
        for (int index = 0; index < modes.Count; index++)
        {
            int mode = (int)modes[index];
            string modeName = Tower.targeting_mode_to_string(mode);
            _targetingModeSelector.AddItem(modeName, mode);
        }
    }

    private void OnTargetingModeSelectorItemSelected(int index)
    {
        if (_currentTower == null)
        {
            return;
        }

        int mode = _targetingModeSelector.GetItemId(index);
        _currentTower.targeting_mode = mode;
    }

    private void OnRemoveButtonPressed()
    {
        if (_currentTower == null)
        {
            return;
        }

        ClickEvents.TowerRemovePressed?.Invoke(_currentTower);
    }

    private void OnUpgradeButtonPressed()
    {
        if (_currentTower == null)
        {
            GD.PushWarning("[TowerStatsPanel] Upgrade pressed with no selected tower");
            return;
        }

        TowerData data = _currentTower.data as TowerData;
        if (data == null)
        {
            return;
        }

        int price = data.upgrade_price;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = runContext.economy.gold;
        if (gold < price)
        {
            GD.Print("[TowerStatsPanel] Not enough gold for upgrade");
            return;
        }

        string towerId = data.id;
        TowerDataWithInstance towerConfiguration = runContext.towers_manager.get_tower_configuration_by_id(towerId);
        if (towerConfiguration == null)
        {
            GD.PushError($"[TowerStatsPanel] Missing tower configuration for id: {towerId}");
            return;
        }

        _currentTower.upgrade();
        ClickEvents.TowerUpgradePressed?.Invoke(_currentTower, towerConfiguration, price);
    }

    private void HideUpgradeOptions()
    {
        _upgradeButtonContainer.Visible = false;
    }

    private void OnUpgradeButtonXarretaMouseEntered()
    {
        if (_currentTower == null)
        {
            return;
        }

        TowerData data = _currentTower.data as TowerData;
        TowerStats statsOnLevel = data?.stats_on_level;
        if (statsOnLevel == null)
        {
            return;
        }

        _damageStat.show_upgrade_value(statsOnLevel.damage);
        _attackSpeedStat.show_upgrade_value(statsOnLevel.attack_speed);
        _rangeStat.show_upgrade_value(statsOnLevel.attack_range);
    }

    private void OnUpgradeButtonXarretaMouseExited()
    {
        _damageStat.hide_upgrade_value();
        _attackSpeedStat.hide_upgrade_value();
        _rangeStat.hide_upgrade_value();
    }
}
