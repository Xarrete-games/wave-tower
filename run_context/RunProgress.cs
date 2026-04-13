using System;

public class RunProgress
{
    public event Action<int> current_wave_changed;
    public event Action current_wave_finished;
    public event Action last_wave_finished;

    private int _currentWave;

    public int current_wave
    {
        get => this._currentWave;
        set
        {
            this._currentWave = value;
            Hooks.OnWaveInit(Hooks.GetListenersFromRuntime());
            this.current_wave_changed?.Invoke(this._currentWave);
        }
    }

    public int total_levels { get; set; }
    public int total_waves { get; set; }

    public bool is_last_wave()
    {
        return this.current_wave >= this.total_waves;
    }

    public void notify_current_wave_finished()
    {
        this.current_wave_finished?.Invoke();
    }

    public void notify_last_wave_finished()
    {
        this.last_wave_finished?.Invoke();
    }
}
