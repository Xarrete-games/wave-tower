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
    public static Action<TowerNode> TowerSelected;
    public static Action<TowerNode> TowerRemovePressed;
    public static Action<TowerNode, TowerDataWithInstance, int> TowerUpgradePressed;
    public static Action<TowerNode> TowerHovered;
    public static Action<TowerNode> TowerUnhovered;
    public static Action<TowerDataWithInstance> AddTowerCard;

    public static bool HasTowerBuildButtonPressedListeners => TowerBuildButtonPressed != null;
    public static bool HasTowerUpgradePressedListeners => TowerUpgradePressed != null;
}
