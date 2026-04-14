using Godot;

public partial class TowerSelector : Node
{
    private Tower _currentTowerSelected;
    private RunProgress _progress;

    public override void _Ready()
    {
        this.ClearTowerSelected();
        ClickEvents.TowerSelected += this.OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed += this.OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed += this.OnTowerRemovePressed;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        this._progress = runContext?.progress;
        if (this._progress != null)
        {
            this._progress.current_wave_finished += this.ClearTowerSelected;
        }
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerSelected -= this.OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed -= this.OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed -= this.OnTowerRemovePressed;

        if (this._progress != null)
        {
            this._progress.current_wave_finished -= this.ClearTowerSelected;
            this._progress = null;
        }
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
        runContext.towers_manager.select_tower(null);
    }

    private void OnTowerSelected(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        if (this._currentTowerSelected == null)
        {
            this._currentTowerSelected = tower;
            this._currentTowerSelected.Connect("stats_change", Callable.From<Variant>(this.OnStatsChange));
        }
        else if (this._currentTowerSelected != tower)
        {
            this._currentTowerSelected.Disconnect("stats_change", Callable.From<Variant>(this.OnStatsChange));
            this._currentTowerSelected = tower;
            this._currentTowerSelected.Connect("stats_change", Callable.From<Variant>(this.OnStatsChange));
        }
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        this.ClearTowerSelected();
    }

    private void OnTowerRemovePressed(Tower tower)
    {
        this.ClearTowerSelected();
    }

    private void OnStatsChange(Variant tower)
    {
        Tower towerObj = tower.AsGodotObject() as Tower;
        if (this._currentTowerSelected == towerObj)
        {
            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            runContext.towers_manager.select_tower(towerObj);
        }
    }
}

