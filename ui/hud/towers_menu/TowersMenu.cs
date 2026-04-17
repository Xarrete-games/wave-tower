using Godot;
using System.Collections.Generic;

public partial class TowersMenu : Control
{
    [Export]
    public Control buttons_container;

    [Export]
    public TowerButtonHint tower_hint;

    [Export]
    public PackedScene tower_button_scene;

    private TowerButton _buttonInHover;
    private readonly Dictionary<string, Node> _buttons = new();
    private TowersManager _towersManager;

    public override void _Ready()
    {
        if (tower_hint != null)
        {
            tower_hint.Visible = false;
        }

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _towersManager = runContext?.towers_manager;
        if (_towersManager != null)
        {
            _towersManager.tower_card_amount_change += OnTowerCardAdded;
        }
        InitButtonCards();
    }

    public override void _ExitTree()
    {
        if (_towersManager != null)
        {
            _towersManager.tower_card_amount_change -= OnTowerCardAdded;
            _towersManager = null;
        }
    }

    private void InitButtonCards()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        foreach (KeyValuePair<string, int> pair in runContext.towers_manager.tower_cards_amount)
        {
            TowerDataWithInstance towerConfiguration = runContext.towers_manager.get_tower_configuration_by_id(pair.Key);
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
            TowerButton towerButton = tower_button_scene.Instantiate<TowerButton>();
            buttons_container.AddChild(towerButton);
            towerButton.tower_data = towerData;
            towerButton.amount = amount;

            towerButton.tower_button_pressed += OnTowerButtonPressed;
            towerButton.hover += OnTowerButtonHover;
            towerButton.unhover += OnTowerButtonUnhover;

            _buttons[towerId] = towerButton;
            return;
        }

        if (buttonNode is TowerButton existingButton)
        {
            existingButton.amount = amount;
        }
    }

    private string GetTowerId(TowerDataWithInstance towerData)
    {
        return towerData?.data?.id ?? string.Empty;
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerData, int price)
    {
        ClickEvents.TowerBuildButtonPressed?.Invoke(towerData, price);
    }

    private void OnTowerButtonHover(TowerButton towerButton)
    {
        if (towerButton == null || tower_hint == null)
        {
            return;
        }

        _buttonInHover = towerButton;
        TowerData data = towerButton.tower_data?.data;
        if (data == null)
        {
            return;
        }

        tower_hint.set_stats(data);

        Rect2 rect = towerButton.GetGlobalRect();
        tower_hint.GlobalPosition = new Vector2(
            rect.Position.X + rect.Size.X * 0.25f - tower_hint.Size.X * 0.5f,
            tower_hint.GlobalPosition.Y
        );
        tower_hint.Visible = true;
    }

    private void OnTowerButtonUnhover(TowerButton towerButton)
    {
        if (towerButton == null || tower_hint == null)
        {
            return;
        }

        if (_buttonInHover == towerButton)
        {
            _buttonInHover = null;
            tower_hint.Visible = false;
        }
    }
}
