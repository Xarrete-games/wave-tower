using Godot;

public partial class TowerPlacer : Node2D
{
    [Export]
    public CompositeTileMap CompositeTileMap;

    private Node2D visual;
    private bool _isPlacing;
    private TowerNode _currentTowerInstance;
    private bool _isValidPlacement;
    private RunProgress _progress;

    public override void _Ready()
    {
        _isPlacing = false;
        visual = GetNode<Node2D>("../Visual");
        ClickEvents.TowerBuildButtonPressed += OnTowerButtonPressed;

        RunContext runContext = RunContext.Instance;
        _progress = runContext.Progress;
        _progress.CurrentWaveFinished += CancelTower;
        _progress.LastWaveFinished += CancelTower;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerBuildButtonPressed -= OnTowerButtonPressed;

        if (_progress != null)
        {
            _progress.CurrentWaveFinished -= CancelTower;
            _progress.LastWaveFinished -= CancelTower;
        }
    }

    public override void _Process(double delta)
    {
        if (!_isPlacing || _currentTowerInstance == null)
        {
            return;
        }

        if (CompositeTileMap.IsMouseOnBuildableTile())
        {
            _isValidPlacement = true;
            _currentTowerInstance.NormalColor();
            _currentTowerInstance.GlobalPosition = CompositeTileMap.GetCurrentTilePos();
        }
        else
        {
            _isValidPlacement = false;
            _currentTowerInstance.PhantomMode();
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
        string key = CompositeTileMap.SetTileOccupiedAtMouse();
        _currentTowerInstance.CompositeTileKey = key;

        _isPlacing = false;
        _currentTowerInstance.Enable();

        RunContext runContext = RunContext.Instance;
        runContext.TowersManager.AddTowerPlaced(_currentTowerInstance);

        _currentTowerInstance = null;

        GetNode<ActionManager>("/root/ActionManager").EndAction();
    }

    private bool HasEnoughGold(int towerPrice)
    {
        RunContext runContext = RunContext.Instance;
        if (runContext.Economy.AvailableFreeTowers > 0)
        {
            return true;
        }

        return runContext.Economy.Gold >= towerPrice;
    }

    private void HandleCosts(int towerPrice)
    {
        RunContext runContext = RunContext.Instance;
        int freeTowers = runContext.Economy.AvailableFreeTowers;
        if (freeTowers > 0)
        {
            runContext.Economy.AvailableFreeTowers = freeTowers - 1;
        }
        else
        {
            runContext.Economy.Gold -= towerPrice;
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

        TowerNode instance = towerConfiguration.GetInstanceNode();
        if (instance == null)
        {
            GD.PushError("[TowerPlacer] get_instance_node did not return a TowerNode.");
            return;
        }

        _currentTowerInstance = instance;
        _currentTowerInstance.BuildPrice = price;
        visual.AddChild(_currentTowerInstance);
        _isPlacing = true;

        GetNode<ActionManager>("/root/ActionManager").StartAction(ActionManager.ActionState.PlacingTower, CancelTower);
    }
}
