using Godot;
using System;

public partial class ClickEvents : Node
{
    public static Action NextWavePressed;
    public static Action NextLevelPressed;
    public static Action ConfigButtonPressed;
    public static Action SpeedButtonPressed;
    public static Action ResetGameButtonPressed;

    public static Action<TowerDataWithInstance, int> TowerBuildButtonPressed;
    public static Action<Variant> TowerSelected;
    public static Action<Variant> TowerRemovePressed;
    public static Action<Variant, Variant, int> TowerUpgradePressed;
    public static Action<Variant> TowerHovered;
    public static Action<Variant> TowerUnhovered;
    public static Action<TowerDataWithInstance> AddTowerCard;

    public static bool HasTowerBuildButtonPressedListeners => TowerBuildButtonPressed != null;
    public static bool HasTowerUpgradePressedListeners => TowerUpgradePressed != null;
}
