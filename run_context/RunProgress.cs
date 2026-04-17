using System;

public class RunProgress
{
    public event Action<int> current_wave_changed;
    public event Action current_wave_finished;
    public event Action last_wave_finished;

    private int _currentWave;

    public int current_wave
    {
        get => _currentWave;
        set
        {
            _currentWave = value;
            Hooks.OnWaveInit(Hooks.GetListenersFromRuntime());
            current_wave_changed?.Invoke(_currentWave);
        }
    }

    public int total_levels { get; set; }
    public int total_waves { get; set; }

    public bool is_last_wave()
    {
        return current_wave >= total_waves;
    }

    public void notify_current_wave_finished()
    {
        current_wave_finished?.Invoke();
    }

    public void notify_last_wave_finished()
    {
        last_wave_finished?.Invoke();
    }
}
