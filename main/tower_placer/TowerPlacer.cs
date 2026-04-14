using Godot;

public partial class TowerPlacer : Node2D
{
    [Export]
    public Node composite_tile_map;

    private Node2D visual;
    private bool _isPlacing;
    private Node2D _currentTowerInstance;
    private bool _isValidPlacement;
    private RunProgress _progress;

    public override void _Ready()
    {
        this._isPlacing = false;
        this.visual = GetNode<Node2D>("../Visual");
        ClickEvents.TowerBuildButtonPressed += this.OnTowerButtonPressed;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext.progress;
        this._progress.current_wave_finished += this.CancelTower;
        this._progress.last_wave_finished += this.CancelTower;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerBuildButtonPressed -= this.OnTowerButtonPressed;

        if (this._progress != null)
        {
            this._progress.current_wave_finished -= this.CancelTower;
            this._progress.last_wave_finished -= this.CancelTower;
        }
    }

    public override void _Process(double delta)
    {
        if (!this._isPlacing || this._currentTowerInstance == null)
        {
            return;
        }

        if ((bool)this.composite_tile_map.Call("is_mouse_on_buildeable_tile"))
        {
            this._isValidPlacement = true;
            this._currentTowerInstance.Call("normal_color");
            this._currentTowerInstance.GlobalPosition = (Vector2)this.composite_tile_map.Call("get_current_tile_pos");
        }
        else
        {
            this._isValidPlacement = false;
            this._currentTowerInstance.Call("phantom_mode");
            this._currentTowerInstance.GlobalPosition = GetGlobalMousePosition();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!this._isPlacing)
        {
            return;
        }

        if (@event is InputEventMouseButton mouseButton
            && mouseButton.ButtonIndex == MouseButton.Left
            && mouseButton.Pressed
            && this._isValidPlacement)
        {
            this.PlaceTower();
        }
    }

    private void PlaceTower()
    {
        if (this._currentTowerInstance == null)
        {
            return;
        }

        int towerPrice = (int)this._currentTowerInstance.Get("build_price");
        if (!this.HasEnoughGold(towerPrice))
        {
            GetNode<ActionManager>("/root/ActionManager").EndAction();
            return;
        }

        this.HandleCosts(towerPrice);
        string key = this.composite_tile_map.Call("set_tile_occupied_at_mouse").AsString();
        this._currentTowerInstance.Set("composite_tile_key", key);

        this._isPlacing = false;
        this._currentTowerInstance.Call("enable");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.add_tower_placed(this._currentTowerInstance);

        this._currentTowerInstance = null;

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
        if (this._currentTowerInstance != null)
        {
            this._currentTowerInstance.QueueFree();
            this._currentTowerInstance = null;
        }

        this._isPlacing = false;
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        if (this._isPlacing)
        {
            return;
        }

        if (towerConfiguration == null)
        {
            GD.PushError("[TowerPlacer] Build payload is null.");
            return;
        }

        Node2D instance = towerConfiguration.get_instance_node() as Node2D;
        if (instance == null)
        {
            GD.PushError("[TowerPlacer] get_instance_node did not return a Node2D tower.");
            return;
        }

        this._currentTowerInstance = instance;
        this._currentTowerInstance.Set("build_price", price);
        this.visual.AddChild(this._currentTowerInstance);
        this._isPlacing = true;

        GetNode<ActionManager>("/root/ActionManager").StartAction(ActionManager.ActionState.PlacingTower, Callable.From(this.CancelTower));
    }
}

