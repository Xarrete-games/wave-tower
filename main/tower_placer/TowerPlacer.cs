using Godot;

public partial class TowerPlacer : Node2D
{
    [Export]
    public CompositeTileMap composite_tile_map;

    private Node2D visual;
    private bool _isPlacing;
    private Tower _currentTowerInstance;
    private bool _isValidPlacement;
    private RunProgress _progress;

    public override void _Ready()
    {
        _isPlacing = false;
        visual = GetNode<Node2D>("../Visual");
        ClickEvents.TowerBuildButtonPressed += OnTowerButtonPressed;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext.progress;
        _progress.current_wave_finished += CancelTower;
        _progress.last_wave_finished += CancelTower;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerBuildButtonPressed -= OnTowerButtonPressed;

        if (_progress != null)
        {
            _progress.current_wave_finished -= CancelTower;
            _progress.last_wave_finished -= CancelTower;
        }
    }

    public override void _Process(double delta)
    {
        if (!_isPlacing || _currentTowerInstance == null)
        {
            return;
        }

        if (composite_tile_map.is_mouse_on_buildeable_tile())
        {
            _isValidPlacement = true;
            _currentTowerInstance.normal_color();
            _currentTowerInstance.GlobalPosition = composite_tile_map.get_current_tile_pos();
        }
        else
        {
            _isValidPlacement = false;
            _currentTowerInstance.phantom_mode();
            _currentTowerInstance.GlobalPosition = GetGlobalMousePosition();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!_isPlacing)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseButton
            && mouseButton.ButtonIndex == MouseButton.Left
            && mouseButton.Pressed
            && _isValidPlacement)
        {
            PlaceTower();
        }
    }

    private void PlaceTower()
    {
        if (_currentTowerInstance == null)
        {
            return;
        }

        int towerPrice = _currentTowerInstance.BuildPrice;
        if (!HasEnoughGold(towerPrice))
        {
            GetNode<ActionManager>("/root/ActionManager").EndAction();
            return;
        }

        HandleCosts(towerPrice);
        string key = composite_tile_map.set_tile_occupied_at_mouse();
        _currentTowerInstance.composite_tile_key = key;

        _isPlacing = false;
        _currentTowerInstance.enable();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.add_tower_placed(_currentTowerInstance);

        _currentTowerInstance = null;

        GetNode<ActionManager>("/root/ActionManager").EndAction();
    }

    private bool HasEnoughGold(int towerPrice)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (runContext.economy.available_free_towers > 0)
        {
            return true;
        }

        return runContext.economy.gold >= towerPrice;
    }

    private void HandleCosts(int towerPrice)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int freeTowers = runContext.economy.available_free_towers;
        if (freeTowers > 0)
        {
            runContext.economy.available_free_towers = freeTowers - 1;
        }
        else
        {
            runContext.economy.gold -= towerPrice;
        }
    }

    private void CancelTower()
    {
        if (_currentTowerInstance != null)
        {
            _currentTowerInstance.QueueFree();
            _currentTowerInstance = null;
        }

        _isPlacing = false;
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        if (_isPlacing)
        {
            return;
        }

        if (towerConfiguration == null)
        {
            GD.PushError("[TowerPlacer] Build payload is null.");
            return;
        }

        Tower instance = towerConfiguration.GetInstanceNode();
        if (instance == null)
        {
            GD.PushError("[TowerPlacer] get_instance_node did not return a Tower.");
            return;
        }

        _currentTowerInstance = instance;
        _currentTowerInstance.BuildPrice = price;
        visual.AddChild(_currentTowerInstance);
        _isPlacing = true;

        GetNode<ActionManager>("/root/ActionManager").StartAction(ActionManager.ActionState.PlacingTower, CancelTower);
    }
}

