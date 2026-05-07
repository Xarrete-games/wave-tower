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
    public static Action<Tower> TowerSelected;
    public static Action<Tower> TowerRemovePressed;
    public static Action<Tower, TowerDataWithInstance, int> TowerUpgradePressed;
    public static Action<Tower> TowerHovered;
    public static Action<Tower> TowerUnhovered;
    public static Action<TowerDataWithInstance> AddTowerCard;

    public static bool HasTowerBuildButtonPressedListeners => TowerBuildButtonPressed != null;
    public static bool HasTowerUpgradePressedListeners => TowerUpgradePressed != null;
}
