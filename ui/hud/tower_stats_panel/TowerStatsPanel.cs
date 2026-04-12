using Godot;
using System.Collections.Generic;

public partial class TowerStatsPanel : Control
{
    private Label _nameLabel;
    private Label _idLabel;
    private Node _damageStat;
    private Node _attackSpeedStat;
    private Node _rangeStat;
    private Label _levelLabel;
    private Control _upgradeButtonContainer;
    private Node _upgradeTowerPrice;
    private OptionButton _targetingModeSelector;
    private Control _towerHintPanel;

    private GodotObject _currentTower;

    public override void _Ready()
    {
        this._nameLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/NameLabel");
        this._idLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/IdLabel");
        this._damageStat = GetNode<Node>("Panel/MarginContainer/MainContainer/DataContainer/DamageStatUi");
        this._attackSpeedStat = GetNode<Node>("Panel/MarginContainer/MainContainer/DataContainer/AttkSpeedStatUi");
        this._rangeStat = GetNode<Node>("Panel/MarginContainer/MainContainer/DataContainer/RangeStatUi");
        this._levelLabel = GetNode<Label>("Panel/MarginContainer/MainContainer/ExpDataContainer/LevelContainer/LevelLabel");
        this._upgradeButtonContainer = GetNode<Control>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer");
        this._upgradeTowerPrice = GetNode<Node>("Panel/MarginContainer/MainContainer/UpgradeButtonContainer/UpgradeTowerPrice");
        this._targetingModeSelector = GetNode<OptionButton>("Panel/MarginContainer/MainContainer/TargetingContainer/TargetingModeSelector");
        this._towerHintPanel = GetNode<Control>("TowerHintPanel");

        this.Visible = false;
        this._towerHintPanel.Visible = false;
        this.HideUpgradeOptions();

        ClickEventsBus.TowerSelected += this.OnTowerSelected;
    }

    public override void _ExitTree()
    {
        ClickEventsBus.TowerSelected -= this.OnTowerSelected;
    }

    private void OnTowerSelected(Variant tower)
    {
        GodotObject towerObj = tower.AsGodotObject();
        if (towerObj == null)
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

        int targetingMode = (int)towerObj.Get("targeting_mode");
        this._targetingModeSelector.Select(targetingMode);

        ActionManager manager = GetNode<ActionManager>("/root/ActionManager");
        manager.StartAction(ActionManager.ActionState.TowerSelected, Callable.From(this.HidePanel));
        this.Visible = true;

        GodotObject stats = towerObj.Get("stats").AsGodotObject();
        GodotObject expData = towerObj.Get("exp_data").AsGodotObject();
        this.UpdateStats(stats);
        this.UpdateExpData(expData);

        GodotObject data = towerObj.Get("data").AsGodotObject();
        this._nameLabel.Text = (string)data.Get("display_name");
        this._idLabel.Text = (string)towerObj.Get("id");
        this._currentTower = towerObj;

        this._levelLabel.Text = ((int)towerObj.Get("level")).ToString();
        if ((bool)towerObj.Call("is_max_level"))
        {
            this.HideUpgradeOptions();
        }
        else
        {
            this._upgradeTowerPrice.Set("price", (int)data.Get("upgrade_price"));
            this._upgradeButtonContainer.Visible = true;
        }
    }

    private void HidePanel()
    {
        this.Visible = false;
    }

    private void UpdateStats(GodotObject towerStats)
    {
        if (towerStats == null)
        {
            return;
        }

        this._damageStat.Call("set_value", towerStats.Get("damage"));
        this._attackSpeedStat.Call("set_value", towerStats.Get("attack_speed"));
        this._rangeStat.Call("set_value", towerStats.Get("attack_range"));
    }

    private void UpdateExpData(GodotObject expData)
    {
        if (expData == null)
        {
            return;
        }

        this._levelLabel.Text = ((int)expData.Get("level")).ToString();
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
        this._currentTower.Set("targeting_mode", mode);
    }

    private void _on_remove_button_pressed()
    {
        if (this._currentTower == null)
        {
            return;
        }

        ClickEventsBus.EmitTowerRemovePressed(this._currentTower);
    }

    private void _on_upgrade_button_pressed()
    {
        if (this._currentTower == null)
        {
            GD.PushWarning("[TowerStatsPanel] Upgrade pressed with no selected tower");
            return;
        }

        GodotObject data = this._currentTower.Get("data").AsGodotObject();
        int price = (int)data.Get("upgrade_price");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int gold = (int)runContext.economy.Get("gold");
        if (gold < price)
        {
            GD.Print("[TowerStatsPanel] Not enough gold for upgrade");
            return;
        }

        string towerId = (string)data.Get("id");
        Variant towerConfiguration = runContext.towers_manager.get_tower_configuration_by_id(towerId);
        if (towerConfiguration.VariantType == Variant.Type.Nil)
        {
            GD.PushError($"[TowerStatsPanel] Missing tower configuration for id: {towerId}");
            return;
        }

        this._currentTower.Call("upgrade");
        ClickEventsBus.EmitTowerUpgradePressed(this._currentTower, towerConfiguration, price);
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

        GodotObject data = this._currentTower.Get("data").AsGodotObject();
        GodotObject statsOnLevel = data.Get("stats_on_level").AsGodotObject();
        this._damageStat.Call("show_upgrade_value", statsOnLevel.Get("damage"));
        this._attackSpeedStat.Call("show_upgrade_value", statsOnLevel.Get("attack_speed"));
        this._rangeStat.Call("show_upgrade_value", statsOnLevel.Get("attack_range"));
    }

    private void _on_upgrade_button_xarreta_mouse_exited()
    {
        this._damageStat.Call("hide_upgrade_value");
        this._attackSpeedStat.Call("hide_upgrade_value");
        this._rangeStat.Call("hide_upgrade_value");
    }
}