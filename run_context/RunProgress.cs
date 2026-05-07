using System;

public class RunProgress
{
    public event Action<int> CurrentWaveChanged;
    public event Action CurrentWaveFinished;
    public event Action LastWaveFinished;

    private int _currentWave;

    public int CurrentWave
    {
        get => _currentWave;
        set
        {
            _currentWave = value;
            Hooks.OnWaveInit(Hooks.GetListenersFromRuntime());
            CurrentWaveChanged?.Invoke(_currentWave);
        }
    }

    public int TotalLevels { get; set; }
    public int TotalWaves { get; set; }

    public bool IsLastWave()
    {
        return CurrentWave >= TotalWaves;
    }

    public void NotifyCurrentWaveFinished()
    {
        CurrentWaveFinished?.Invoke();
    }

    public void NotifyLastWaveFinished()
    {
        LastWaveFinished?.Invoke();
    }
}
