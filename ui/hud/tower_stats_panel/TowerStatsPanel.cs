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

        int targetingMode = tower.CurrentTargetingMode;
        _targetingModeSelector.Select(targetingMode);

        ActionManager manager = GetNode<ActionManager>("/root/ActionManager");
        manager.StartAction(ActionManager.ActionState.TowerSelected, HidePanel);
        Visible = true;

        TowerStats stats = tower.Stats;
        TowerExpData expData = tower.ExpData;
        UpdateStats(stats);
        UpdateExpData(expData);

        TowerData data = tower.Data as TowerData;
        _nameLabel.Text = data?.DisplayName ?? string.Empty;
        _idLabel.Text = tower.Id;
        _currentTower = tower;

        _levelLabel.Text = tower.Level.ToString();
        if (tower.IsMaxLevel())
        {
            HideUpgradeOptions();
        }
        else
        {
            _upgradeTowerPrice.Price = data?.UpgradePrice ?? 0;
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

        _damageStat.SetValue(towerStats.Damage);
        _attackSpeedStat.SetValue(towerStats.AttackSpeed);
        _rangeStat.SetValue(towerStats.AttackRange);
    }

    private void UpdateExpData(TowerExpData expData)
    {
        if (expData == null)
        {
            return;
        }

        _levelLabel.Text = expData.Level.ToString();
    }

    private void UpdateTargetingModes(List<TowerTargetingMode> modes)
    {
        _targetingModeSelector.Clear();
        for (int index = 0; index < modes.Count; index++)
        {
            int mode = (int)modes[index];
            string modeName = Tower.TargetingModeToString(mode);
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
        _currentTower.CurrentTargetingMode = mode;
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

        TowerData data = _currentTower.Data as TowerData;
        if (data == null)
        {
            return;
        }

        int price = data.UpgradePrice;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = runContext.Economy.Gold;
        if (gold < price)
        {
            GD.Print("[TowerStatsPanel] Not enough gold for upgrade");
            return;
        }

        string towerId = data.Id;
        TowerDataWithInstance towerConfiguration = runContext.TowersManager.GetTowerConfigurationById(towerId);
        if (towerConfiguration == null)
        {
            GD.PushError($"[TowerStatsPanel] Missing tower configuration for id: {towerId}");
            return;
        }

        _currentTower.Upgrade();
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

        TowerData data = _currentTower.Data as TowerData;
        TowerStats statsOnLevel = data?.StatsOnLevel;
        if (statsOnLevel == null)
        {
            return;
        }

        _damageStat.ShowUpgradeValue(statsOnLevel.Damage);
        _attackSpeedStat.ShowUpgradeValue(statsOnLevel.AttackSpeed);
        _rangeStat.ShowUpgradeValue(statsOnLevel.AttackRange);
    }

    private void OnUpgradeButtonXarretaMouseExited()
    {
        _damageStat.HideUpgradeValue();
        _attackSpeedStat.HideUpgradeValue();
        _rangeStat.HideUpgradeValue();
    }
}
