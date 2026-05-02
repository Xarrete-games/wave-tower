using Godot;
using System.Collections.Generic;

public partial class TowersMenu : Control
{
    [Export]
    public Control ButtonsContainer;

    [Export]
    public TowerButtonHint TowerHint;

    [Export]
    public PackedScene TowerButtonScene;

    private TowerButton _buttonInHover;
    private readonly Dictionary<string, Node> _buttons = new();
    private TowersManager _towersManager;

    public override void _Ready()
    {
        if (TowerHint != null)
        {
            TowerHint.Visible = false;
        }

        RunContext runContext = RunContext.Instance;
        _towersManager = runContext.TowersManager;
        _towersManager.TowerCardAmountChanged += OnTowerCardAdded;
        InitButtonCards();
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.TowerCardAmountChanged -= OnTowerCardAdded;
            _towersManager = null;
        }
    }

    private void InitButtonCards()
    {
        RunContext runContext = RunContext.Instance;
        foreach (KeyValuePair<string, int> pair in runContext.TowersManager.TowerCardsAmount)
        {
            TowerDataWithInstance towerConfiguration = runContext.TowersManager.GetTowerConfigurationById(pair.Key);
            OnTowerCardAdded(towerConfiguration, pair.Value);
        }
    }

    private void OnTowerCardAdded(TowerDataWithInstance towerData, int amount)
    {
        string towerId = GetTowerId(towerData);
        if (string.IsNullOrEmpty(towerId))
        {
            return;
        }

        if (amount == 0)
        {
            if (_buttons.TryGetValue(towerId, out Node nodeToRemove))
            {
                nodeToRemove.QueueFree();
                _buttons.Remove(towerId);
            }

            return;
        }

        if (!_buttons.TryGetValue(towerId, out Node buttonNode))
        {
            TowerButton towerButton = TowerButtonScene.Instantiate<TowerButton>();
            ButtonsContainer.AddChild(towerButton);
            towerButton.TowerData = towerData;
            towerButton.Amount = amount;

            towerButton.TowerButtonPressed += OnTowerButtonPressed;
            towerButton.Hover += OnTowerButtonHover;
            towerButton.Unhover += OnTowerButtonUnhover;

            _buttons[towerId] = towerButton;
            return;
        }

        if (buttonNode is TowerButton existingButton)
        {
            existingButton.Amount = amount;
        }
    }

    private string GetTowerId(TowerDataWithInstance towerData)
    {
        return towerData?.Data?.Id ?? string.Empty;
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerData, int price)
    {
        ClickEvents.TowerBuildButtonPressed?.Invoke(towerData, price);
    }

    private void OnTowerButtonHover(TowerButton towerButton)
    {
        if (towerButton == null || TowerHint == null)
        {
            return;
        }

        _buttonInHover = towerButton;
        TowerData data = towerButton.TowerData?.Data;
        if (data == null)
        {
            return;
        }

        TowerHint.SetStats(data);

        Rect2 rect = towerButton.GetGlobalRect();
        TowerHint.GlobalPosition = new Vector2(
            rect.Position.X + rect.Size.X * 0.25f - TowerHint.Size.X * 0.5f,
            TowerHint.GlobalPosition.Y
        );
        TowerHint.Visible = true;
    }

    private void OnTowerButtonUnhover(TowerButton towerButton)
    {
        if (towerButton == null || TowerHint == null)
        {
            return;
        }

        if (_buttonInHover == towerButton)
        {
            _buttonInHover = null;
            TowerHint.Visible = false;
        }
    }
}

