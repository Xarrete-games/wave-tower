using Godot;

public partial class TowerSelector : Node
{
    private TowerNode _currentTowerSelected;
    private RunProgress _progress;

    public override void _Ready()
    {
        ClearTowerSelected();
        ClickEvents.TowerSelected += OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed += OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed += OnTowerRemovePressed;

        RunContext runContext = RunContext.Instance;
        _progress = runContext.Progress;
        _progress.CurrentWaveFinished += ClearTowerSelected;
    }

    public override void _ExitTree()
    {
        ClickEvents.TowerSelected -= OnTowerSelected;
        ClickEvents.TowerBuildButtonPressed -= OnTowerButtonPressed;
        ClickEvents.TowerRemovePressed -= OnTowerRemovePressed;

        if (_progress != null)
        {
            _progress.CurrentWaveFinished -= ClearTowerSelected;
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
            _currentTowerSelected.StatsChanged -= OnStatsChange;
        }

        _currentTowerSelected = null;
        RunContext runContext = RunContext.Instance;
        runContext.TowersManager.SelectTower(null);
    }

    private void OnTowerSelected(TowerNode tower)
    {
        if (tower == null)
        {
            return;
        }

        if (_currentTowerSelected == null)
        {
            _currentTowerSelected = tower;
            _currentTowerSelected.StatsChanged += OnStatsChange;
        }
        else if (_currentTowerSelected != tower)
        {
            _currentTowerSelected.StatsChanged -= OnStatsChange;
            _currentTowerSelected = tower;
            _currentTowerSelected.StatsChanged += OnStatsChange;
        }
    }

    private void OnTowerButtonPressed(TowerDataWithInstance towerConfiguration, int price)
    {
        ClearTowerSelected();
    }

    private void OnTowerRemovePressed(TowerNode tower)
    {
        ClearTowerSelected();
    }

    private void OnStatsChange(TowerNode towerObj)
    {
        if (_currentTowerSelected == towerObj)
        {
            RunContext runContext = RunContext.Instance;
            runContext.TowersManager.SelectTower(towerObj);
        }
    }
}

