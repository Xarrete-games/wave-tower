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
        this._nameLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/NameLabel");
        this._idLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/IdLabel");
        this._damageStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/DamageStatUi");
        this._attackSpeedStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/AttkSpeedStatUi");
        this._rangeStat = GetNode<TowerStatUi>("Panel/MarginContainer/MainContainer/DataContainer/RangeStatUi");
        this._levelLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/ExpDataContainer/LevelContainer/LevelLabel");
        this._upgradeButtonContainer = GetNode<Control>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer");
        this._upgradeTowerPrice = GetNode<GoldPrice>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer/UpgradeTowerPrice");
        this._targetingModeSelector = GetNode<OptionButton>("Panel/MarginContainer/MainContainer/TargetingContainer/TargetingModeSelector");
        this._towerHintPanel = GetNode<Control>("TowerHintPanel");

        this.Visible = false;
        this._towerHintPanel.Visible = false;
        this.HideUpgradeOptions();

        ClickEvents.TowerSelected += this.OnTowerSelected;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerSelected -= this.OnTowerSelected;
    }

    private void OnTowerSelected(Tower tower)
    {
        if (tower == null)
        {
            this._currentTower = null;
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
        this.UpdateTargetingModes(targetingModes);

        int targetingMode = tower.targeting_mode;
        this._targetingModeSelector.Select(targetingMode);

        ActionManager manager = GetNode<ActionManager>("/root/ActionManager");
        manager.StartAction(ActionManager.ActionState.TowerSelected, this.HidePanel);
        this.Visible = true;

        TowerStats stats = tower.stats;
        TowerExpData expData = tower.exp_data;
        this.UpdateStats(stats);
        this.UpdateExpData(expData);

        TowerData data = tower.data as TowerData;
        this._nameLabel.Text = data?.display_name ?? string.Empty;
        this._idLabel.Text = tower.id;
        this._currentTower = tower;

        this._levelLabel.Text = tower.level.ToString();
        if (tower.is_max_level())
        {
            this.HideUpgradeOptions();
        }
        else
        {
            this._upgradeTowerPrice.price = data?.upgrade_price ?? 0;
            this._upgradeButtonContainer.Visible = true;
        }
    }

    private void HidePanel()
    {
        this.Visible = false;
    }

    private void UpdateStats(TowerStats towerStats)
    {
        if (towerStats == null)
        {
            return;
        }

        this._damageStat.set_value(towerStats.damage);
        this._attackSpeedStat.set_value(towerStats.attack_speed);
        this._rangeStat.set_value(towerStats.attack_range);
    }

    private void UpdateExpData(TowerExpData expData)
    {
        if (expData == null)
        {
            return;
        }

        this._levelLabel.Text = expData.level.ToString();
    }

    private void UpdateTargetingModes(List<TowerTargetingMode> modes)
    {
        this._targetingModeSelector.Clear();
        for (int index = 0; index < modes.Count; index++)
        {
            int mode = (int)modes[index];
            string modeName = Tower.targeting_mode_to_string(mode);
            this._targetingModeSelector.AddItem(modeName, mode);
        }
    }

    private void _on_targeting_mode_selector_item_selected(int index)
    {
        if (this._currentTower == null)
        {
            return;
        }

        int mode = this._targetingModeSelector.GetItemId(index);
        this._currentTower.targeting_mode = mode;
    }

    private void _on_remove_button_pressed()
    {
        if (this._currentTower == null)
        {
            return;
        }

        ClickEvents.TowerRemovePressed?.Invoke(this._currentTower);
    }

    private void _on_upgrade_button_pressed()
    {
        if (this._currentTower == null)
        {
            GD.PushWarning("[TowerStatsPanel] Upgrade pressed with no selected tower");
            return;
        }

        TowerData data = this._currentTower.data as TowerData;
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

        this._currentTower.upgrade();
        ClickEvents.TowerUpgradePressed?.Invoke(this._currentTower, towerConfiguration, price);
    }

    private void HideUpgradeOptions()
    {
        this._upgradeButtonContainer.Visible = false;
    }

    private void _on_upgrade_button_xarreta_mouse_entered()
    {
        if (this._currentTower == null)
        {
            return;
        }

        TowerData data = this._currentTower.data as TowerData;
        TowerStats statsOnLevel = data?.stats_on_level;
        if (statsOnLevel == null)
        {
            return;
        }

        this._damageStat.show_upgrade_value(statsOnLevel.damage);
        this._attackSpeedStat.show_upgrade_value(statsOnLevel.attack_speed);
        this._rangeStat.show_upgrade_value(statsOnLevel.attack_range);
    }

    private void _on_upgrade_button_xarreta_mouse_exited()
    {
        this._damageStat.hide_upgrade_value();
        this._attackSpeedStat.hide_upgrade_value();
        this._rangeStat.hide_upgrade_value();
    }
}
