using Godot;

public partial class TowerSelector : Node
{
    private Tower _currentTowerSelected;
    private RunProgress _progress;

    public override void _Ready()
    {
        ClearTowerSelected();
        ClickEvents.TowerSelected += OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed += OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed += OnTowerRemovePressed;

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext?.progress;
        if (_progress != null)
        {
            _progress.current_wave_finished += ClearTowerSelected;
        }
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerSelected -= OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed -= OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed -= OnTowerRemovePressed;

        if (_progress != null)
        {
            _progress.current_wave_finished -= ClearTowerSelected;
            _progress = null;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (InputClickUtils.IsRightClickReleased(@event))
        {
            ClearTowerSelected();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (InputClickUtils.IsLeftClickReleased(@event))
        {
            ClearTowerSelected();
            GetViewport().SetInputAsHandled();
        }
    }

    private void ClearTowerSelected()
    {
        if (_currentTowerSelected != null)
        {
            _currentTowerSelected.stats_changed -= OnStatsChange;
        }

        _currentTowerSelected = null;
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.towers_manager.select_tower(null);
    }

    private void OnTowerSelected(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        if (_currentTowerSelected == null)
        {
            _currentTowerSelected = tower;
            _currentTowerSelected.stats_changed += OnStatsChange;
        }
        else if (_currentTowerSelected != tower)
        {
            _currentTowerSelected.stats_changed -= OnStatsChange;
            _currentTowerSelected = tower;
            _currentTowerSelected.stats_changed += OnStatsChange;
        }
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        ClearTowerSelected();
    }

    private void OnTowerRemovePressed(Tower tower)
    {
        ClearTowerSelected();
    }

    private void OnStatsChange(Tower towerObj)
    {
        if (_currentTowerSelected == towerObj)
        {
            RunContext runContext = GetNode<RunContext>("/root/RunContext");
            runContext.towers_manager.select_tower(towerObj);
        }
    }
}

