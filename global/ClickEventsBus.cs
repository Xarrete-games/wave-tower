using System;
using Godot;

public static class ClickEventsBus
{
    public static event Action NextWavePressed;
    public static event Action NextLevelPressed;
    public static event Action ConfigButtonPressed;
    public static event Action SpeedButtonPressed;
    public static event Action ResetGameButtonPressed;

    public static event Action<TowerDataWithInstance, int> TowerBuildButtonPressed;
    public static event Action<Variant> TowerSelected;
    public static event Action<Variant> TowerRemovePressed;
    public static event Action<Variant, Variant, int> TowerUpgradePressed;
    public static event Action<Variant> TowerHovered;
    public static event Action<Variant> TowerUnhovered;
    public static event Action<Variant> AddTowerCard;

    public static bool HasTowerBuildButtonPressedListeners => TowerBuildButtonPressed != null;
    public static bool HasTowerUpgradePressedListeners => TowerUpgradePressed != null;

    public static void EmitNextWavePressed()
    {
        NextWavePressed?.Invoke();
    }

    public static void EmitNextLevelPressed()
    {
        NextLevelPressed?.Invoke();
    }

    public static void EmitConfigButtonPressed()
    {
        ConfigButtonPressed?.Invoke();
    }

    public static void EmitSpeedButtonPressed()
    {
        SpeedButtonPressed?.Invoke();
    }

    public static void EmitResetGameButtonPressed()
    {
        ResetGameButtonPressed?.Invoke();
    }

    public static void EmitTowerBuildButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        TowerBuildButtonPressed?.Invoke(towerConfiguration, price);
    }

    public static void EmitTowerSelected(Variant tower)
    {
        TowerSelected?.Invoke(tower);
    }

    public static void EmitTowerRemovePressed(Variant tower)
    {
        TowerRemovePressed?.Invoke(tower);
    }

    public static void EmitTowerUpgradePressed(Variant tower, Variant towerToUpgrade, int price)
    {
        TowerUpgradePressed?.Invoke(tower, towerToUpgrade, price);
    }

    public static void EmitTowerHovered(Variant tower)
    {
        TowerHovered?.Invoke(tower);
    }

    public static void EmitTowerUnhovered(Variant tower)
    {
        TowerUnhovered?.Invoke(tower);
    }

    public static void EmitAddTowerCard(Variant towerConfiguration)
    {
        AddTowerCard?.Invoke(towerConfiguration);
    }
}