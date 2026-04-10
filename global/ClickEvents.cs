using Godot;

public partial class ClickEvents : Node
{
    public static ClickEvents Instance { get; private set; }

    [Signal]
    public delegate void tower_build_button_pressedEventHandler(Variant tower_configuration, int price);

    [Signal]
    public delegate void next_wave_pressedEventHandler();

    [Signal]
    public delegate void next_level_pressedEventHandler();

    [Signal]
    public delegate void config_button_pressedEventHandler();

    [Signal]
    public delegate void speed_button_pressedEventHandler();

    [Signal]
    public delegate void reset_game_button_pressedEventHandler();

    [Signal]
    public delegate void tower_selectedEventHandler(Variant tower);

    [Signal]
    public delegate void tower_remove_pressedEventHandler(Variant tower);

    [Signal]
    public delegate void tower_hoveredEventHandler(Variant tower);

    [Signal]
    public delegate void tower_unhoveredEventHandler(Variant tower);

    [Signal]
    public delegate void tower_upgrade_pressedEventHandler(Variant tower, Variant tower_to_upgrade, int price);

    [Signal]
    public delegate void level_progess_hoveredEventHandler();

    [Signal]
    public delegate void level_progess_unhoveredEventHandler();

    [Signal]
    public delegate void add_tower_cardEventHandler(Variant tower_configuration);

    public override void _EnterTree()
    {
        Instance = this;
        ClickEventsBus.NextWavePressed += this.OnNextWavePressed;
        ClickEventsBus.NextLevelPressed += this.OnNextLevelPressed;
        ClickEventsBus.ConfigButtonPressed += this.OnConfigButtonPressed;
        ClickEventsBus.SpeedButtonPressed += this.OnSpeedButtonPressed;
        ClickEventsBus.ResetGameButtonPressed += this.OnResetGameButtonPressed;

        ClickEventsBus.TowerBuildButtonPressed += this.OnTowerBuildButtonPressed;
        ClickEventsBus.TowerSelected += this.OnTowerSelected;
        ClickEventsBus.TowerRemovePressed += this.OnTowerRemovePressed;
        ClickEventsBus.TowerUpgradePressed += this.OnTowerUpgradePressed;
        ClickEventsBus.TowerHovered += this.OnTowerHovered;
        ClickEventsBus.TowerUnhovered += this.OnTowerUnhovered;
        ClickEventsBus.AddTowerCard += this.OnAddTowerCard;
    }

    public override void _ExitTree()
    {
        ClickEventsBus.NextWavePressed -= this.OnNextWavePressed;
        ClickEventsBus.NextLevelPressed -= this.OnNextLevelPressed;
        ClickEventsBus.ConfigButtonPressed -= this.OnConfigButtonPressed;
        ClickEventsBus.SpeedButtonPressed -= this.OnSpeedButtonPressed;
        ClickEventsBus.ResetGameButtonPressed -= this.OnResetGameButtonPressed;

        ClickEventsBus.TowerBuildButtonPressed -= this.OnTowerBuildButtonPressed;
        ClickEventsBus.TowerSelected -= this.OnTowerSelected;
        ClickEventsBus.TowerRemovePressed -= this.OnTowerRemovePressed;
        ClickEventsBus.TowerUpgradePressed -= this.OnTowerUpgradePressed;
        ClickEventsBus.TowerHovered -= this.OnTowerHovered;
        ClickEventsBus.TowerUnhovered -= this.OnTowerUnhovered;
        ClickEventsBus.AddTowerCard -= this.OnAddTowerCard;

        if (ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }

    public void emit_tower_build_button_pressed(Variant tower_configuration, int price)
    {
        GD.Print($"[ClickEvents] emit_tower_build_button_pressed price={price} hasListeners={ClickEventsBus.HasTowerBuildButtonPressedListeners}");
        ClickEventsBus.EmitTowerBuildButtonPressed(tower_configuration, price);
    }

    public void emit_next_wave_pressed()
    {
        ClickEventsBus.EmitNextWavePressed();
    }

    public void emit_next_level_pressed()
    {
        ClickEventsBus.EmitNextLevelPressed();
    }

    public void emit_config_button_pressed()
    {
        ClickEventsBus.EmitConfigButtonPressed();
    }

    public void emit_speed_button_pressed()
    {
        ClickEventsBus.EmitSpeedButtonPressed();
    }

    public void emit_reset_game_button_pressed()
    {
        ClickEventsBus.EmitResetGameButtonPressed();
    }

    public void emit_add_tower_card(Variant tower_configuration)
    {
        ClickEventsBus.EmitAddTowerCard(tower_configuration);
    }

    public void emit_tower_selected(Variant tower)
    {
        ClickEventsBus.EmitTowerSelected(tower);
    }

    public void emit_tower_remove_pressed(Variant tower)
    {
        ClickEventsBus.EmitTowerRemovePressed(tower);
    }

    public void emit_tower_upgrade_pressed(Variant tower, Variant tower_to_upgrade, int price)
    {
        GD.Print($"[ClickEvents] emit_tower_upgrade_pressed price={price} hasListeners={ClickEventsBus.HasTowerUpgradePressedListeners}");
        ClickEventsBus.EmitTowerUpgradePressed(tower, tower_to_upgrade, price);
    }

    public void emit_tower_hovered(Variant tower)
    {
        ClickEventsBus.EmitTowerHovered(tower);
    }

    public void emit_tower_unhovered(Variant tower)
    {
        ClickEventsBus.EmitTowerUnhovered(tower);
    }

    private void OnTowerBuildButtonPressed(Variant towerConfiguration, int price)
    {
        EmitSignal(SignalName.tower_build_button_pressed, towerConfiguration, price);
    }

    private void OnNextWavePressed()
    {
        EmitSignal(SignalName.next_wave_pressed);
    }

    private void OnNextLevelPressed()
    {
        EmitSignal(SignalName.next_level_pressed);
    }

    private void OnConfigButtonPressed()
    {
        EmitSignal(SignalName.config_button_pressed);
    }

    private void OnSpeedButtonPressed()
    {
        EmitSignal(SignalName.speed_button_pressed);
    }

    private void OnResetGameButtonPressed()
    {
        EmitSignal(SignalName.reset_game_button_pressed);
    }

    private void OnTowerSelected(Variant tower)
    {
        EmitSignal(SignalName.tower_selected, tower);
    }

    private void OnTowerRemovePressed(Variant tower)
    {
        EmitSignal(SignalName.tower_remove_pressed, tower);
    }

    private void OnTowerUpgradePressed(Variant tower, Variant towerToUpgrade, int price)
    {
        EmitSignal(SignalName.tower_upgrade_pressed, tower, towerToUpgrade, price);
    }

    private void OnTowerHovered(Variant tower)
    {
        EmitSignal(SignalName.tower_hovered, tower);
    }

    private void OnTowerUnhovered(Variant tower)
    {
        EmitSignal(SignalName.tower_unhovered, tower);
    }

    private void OnAddTowerCard(Variant towerConfiguration)
    {
        EmitSignal(SignalName.add_tower_card, towerConfiguration);
    }
}
