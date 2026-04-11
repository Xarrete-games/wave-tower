using Godot;

public partial class TowerSelector : Node
{
    private GodotObject _currentTowerSelected;

    public override void _Ready()
    {
        this.ClearTowerSelected();
        ClickEventsBus.TowerSelected += this.OnTowerSelected;
        ClickEventsBus.TowerBuildButtonPressed += this.OnTowerButtonPressed;
        ClickEventsBus.TowerRemovePressed += this.OnTowerRemovePressed;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.progress.Connect("current_wave_finished", Callable.From(this.ClearTowerSelected));
    }

    public override void _ExitTree()
    {
        ClickEventsBus.TowerSelected -= this.OnTowerSelected;
        ClickEventsBus.TowerBuildButtonPressed -= this.OnTowerButtonPressed;
        ClickEventsBus.TowerRemovePressed -= this.OnTowerRemovePressed;
    }

    public override void _Input(InputEvent @event)
    {
        if (InputClickUtils.IsRightClickReleased(@event))
        {
            this.ClearTowerSelected();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (InputClickUtils.IsLeftClickReleased(@event))
        {
            this.ClearTowerSelected();
            GetViewport().SetInputAsHandled();
        }
    }

    private void ClearTowerSelected()
    {
        if (this._currentTowerSelected != null)
        {
            this._currentTowerSelected.Disconnect("stats_change", Callable.From<Variant>(this.OnStatsChange));
        }

        this._currentTowerSelected = null;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.Call("select_tower", default(Variant));
    }

    private void OnTowerSelected(Variant tower)
    {
        GodotObject towerObj = tower.AsGodotObject();
        if (towerObj == null)
        {
            return;
        }

        if (this._currentTowerSelected == null)
        {
            this._currentTowerSelected = towerObj;
            this._currentTowerSelected.Connect("stats_change", Callable.From<Variant>(this.OnStatsChange));
        }
        else if (this._currentTowerSelected != towerObj)
        {
            this._currentTowerSelected.Disconnect("stats_change", Callable.From<Variant>(this.OnStatsChange));
            this._currentTowerSelected = towerObj;
            this._currentTowerSelected.Connect("stats_change", Callable.From<Variant>(this.OnStatsChange));
        }
    }

    private void OnTowerButtonPressed(Variant towerConfiguration, int price)
    {
        this.ClearTowerSelected();
    }

    private void OnTowerRemovePressed(Variant tower)
    {
        this.ClearTowerSelected();
    }

    private void OnStatsChange(Variant tower)
    {
        if (this._currentTowerSelected == tower.AsGodotObject())
        {
            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            runContext.towers_manager.Call("select_tower", tower);
        }
    }
}
