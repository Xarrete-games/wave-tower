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
    }

    public override void _ExitTree()
    {
        if (ReferenceEquals(Instance, this))
        {
            Instance = null;
        }
    }

    public void emit_tower_build_button_pressed(Variant tower_configuration, int price)
    {
        GD.Print($"[ClickEvents] emit_tower_build_button_pressed price={price} hasListeners={ClickEventsBus.HasTowerBuildButtonPressedListeners}");
        ClickEventsBus.EmitTowerBuildButtonPressed(tower_configuration, price);
        EmitSignal(SignalName.tower_build_button_pressed, tower_configuration, price);
    }

    public void emit_tower_selected(Variant tower)
    {
        ClickEventsBus.EmitTowerSelected(tower);
        EmitSignal(SignalName.tower_selected, tower);
    }

    public void emit_tower_remove_pressed(Variant tower)
    {
        ClickEventsBus.EmitTowerRemovePressed(tower);
        EmitSignal(SignalName.tower_remove_pressed, tower);
    }

    public void emit_tower_upgrade_pressed(Variant tower, Variant tower_to_upgrade, int price)
    {
        GD.Print($"[ClickEvents] emit_tower_upgrade_pressed price={price} hasListeners={ClickEventsBus.HasTowerUpgradePressedListeners}");
        ClickEventsBus.EmitTowerUpgradePressed(tower, tower_to_upgrade, price);
        EmitSignal(SignalName.tower_upgrade_pressed, tower, tower_to_upgrade, price);
    }

    public void emit_tower_hovered(Variant tower)
    {
        ClickEventsBus.EmitTowerHovered(tower);
        EmitSignal(SignalName.tower_hovered, tower);
    }

    public void emit_tower_unhovered(Variant tower)
    {
        ClickEventsBus.EmitTowerUnhovered(tower);
        EmitSignal(SignalName.tower_unhovered, tower);
    }
}
