using Godot;

public partial class TowerPlacer : Node2D
{
    [Export]
    public Node composite_tile_map;

    private Node2D visual;
    private bool _isPlacing;
    private Node2D _currentTowerInstance;
    private bool _isValidPlacement;
    private Node2D _towerToUpgrade;
    private bool _isUpgradePlacement;

    public override void _Ready()
    {
        this._isPlacing = false;
        this.visual = GetNode<Node2D>("../Visual");
        ClickEventsBus.TowerBuildButtonPressed += this.OnTowerButtonPressed;
        ClickEventsBus.TowerUpgradePressed += this.OnTowerUpgradePressed;
        GD.Print("[TowerPlacer] Subscribed to ClickEventsBus delegates");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.Connect("current_wave_finished", Callable.From(this.CancelTower));
        runContext.progress.Connect("last_wave_finished", Callable.From(this.CancelTower));
    }

    public override void _ExitTree()
    {
        ClickEventsBus.TowerBuildButtonPressed -= this.OnTowerButtonPressed;
        ClickEventsBus.TowerUpgradePressed -= this.OnTowerUpgradePressed;
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
        if (!this.CanAffordCurrentPlacement(towerPrice))
        {
            GetNode<ActionManager>("/root/ActionManager").EndAction();
            return;
        }

        this.ApplyCurrentPlacementCost(towerPrice);
        string key = this.composite_tile_map.Call("set_tile_occupied_at_mouse").AsString();
        this._currentTowerInstance.Set("composite_tile_key", key);

        this._isPlacing = false;
        this._currentTowerInstance.Call("enable");

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.Call("add_tower_placed", this._currentTowerInstance);

        if (this._isUpgradePlacement && this._towerToUpgrade != null)
        {
            this._currentTowerInstance.Call("copy_tower_data", this._towerToUpgrade);
            runContext.towers_manager.Call("tower_removed", this._towerToUpgrade);
            ClickEvents.Instance?.emit_tower_selected(this._currentTowerInstance);
        }

        this._currentTowerInstance = null;
        this._towerToUpgrade = null;
        this._isUpgradePlacement = false;

        GetNode<ActionManager>("/root/ActionManager").EndAction();
    }

    private bool HasEnoughGold(int towerPrice)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if ((int)runContext.economy.Get("available_free_towers") > 0)
        {
            return true;
        }

        return (int)runContext.economy.Get("gold") >= towerPrice;
    }

    private void HandleCosts(int towerPrice)
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        int freeTowers = (int)runContext.economy.Get("available_free_towers");
        if (freeTowers > 0)
        {
            runContext.economy.Set("available_free_towers", freeTowers - 1);
        }
        else
        {
            int gold = (int)runContext.economy.Get("gold");
            runContext.economy.Set("gold", gold - towerPrice);
        }
    }

    private void CancelTower()
    {
        GD.Print($"[TowerPlacer] CancelTower called. hadInstance={this._currentTowerInstance != null} isPlacing={this._isPlacing}");
        if (this._currentTowerInstance != null)
        {
            this._currentTowerInstance.QueueFree();
            this._currentTowerInstance = null;
        }

        this._isPlacing = false;
        this._towerToUpgrade = null;
        this._isUpgradePlacement = false;
    }

    private void OnTowerButtonPressed(Variant towerConfiguration, int price)
    {
        GD.Print($"[TowerPlacer] OnTowerButtonPressed received price={price} placing={this._isPlacing}");
        if (this._isPlacing)
        {
            return;
        }

        GodotObject towerConfigurationObj = towerConfiguration.AsGodotObject();
        if (towerConfigurationObj == null)
        {
            GD.PushError("[TowerPlacer] Build payload is not a GodotObject configuration.");
            return;
        }

        Node2D instance = towerConfigurationObj.Call("get_instance").As<Node2D>();
        if (instance == null)
        {
            GD.PushError("[TowerPlacer] get_instance did not return a Node2D tower.");
            return;
        }

        this._currentTowerInstance = instance;
        this._isUpgradePlacement = false;
        this._towerToUpgrade = null;
        this._currentTowerInstance.Set("build_price", price);
        this.visual.AddChild(this._currentTowerInstance);
        this._isPlacing = true;
        GD.Print("[TowerPlacer] Build placement started");

        GetNode<ActionManager>("/root/ActionManager").StartAction(ActionManager.ActionState.PlacingTower, Callable.From(this.CancelTower));
    }

    private void OnTowerUpgradePressed(Variant currentTower, Variant newTowerConf, int price)
    {
        if (this._isPlacing)
        {
            return;
        }

        GD.Print($"[TowerPlacer] OnTowerUpgradePressed received price={price}");

        Node2D currentTowerNode = currentTower.As<Node2D>();
        if (currentTowerNode == null)
        {
            GD.PushError("TowerUpgradePressed received invalid current tower payload.");
            return;
        }

        GodotObject newTowerConfObj = newTowerConf.AsGodotObject();
        if (newTowerConfObj == null)
        {
            GD.PushError("TowerUpgradePressed received invalid new tower configuration payload.");
            return;
        }

        Node2D newTower = newTowerConfObj.Call("get_instance").As<Node2D>();
        if (newTower == null)
        {
            GD.PushError("TowerUpgradePressed could not instantiate upgraded tower.");
            return;
        }

        this._currentTowerInstance = newTower;
        this._currentTowerInstance.Set("build_price", price);
        this._towerToUpgrade = currentTowerNode;
        this._isUpgradePlacement = true;
        this.visual.AddChild(this._currentTowerInstance);
        this._isPlacing = true;

        GetNode<ActionManager>("/root/ActionManager").StartAction(ActionManager.ActionState.PlacingTower, Callable.From(this.CancelTower));
    }

    private bool CanAffordCurrentPlacement(int towerPrice)
    {
        if (this._isUpgradePlacement)
        {
            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            return (int)runContext.economy.Get("gold") >= towerPrice;
        }

        return this.HasEnoughGold(towerPrice);
    }

    private void ApplyCurrentPlacementCost(int towerPrice)
    {
        if (this._isUpgradePlacement)
        {
            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            int gold = (int)runContext.economy.Get("gold");
            runContext.economy.Set("gold", gold - towerPrice);
            return;
        }

        this.HandleCosts(towerPrice);
    }
}
