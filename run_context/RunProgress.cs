using Godot;

[GlobalClass]
public partial class RunProgress : RefCounted
{
    [Signal]
    public delegate void current_wave_changedEventHandler(int wave_num);

    [Signal]
    public delegate void current_wave_finishedEventHandler();

    [Signal]
    public delegate void last_wave_finishedEventHandler();

    private int _currentWave;

    public int current_wave
    {
        get => this._currentWave;
        set
        {
            this._currentWave = value;
            Hooks.OnWaveInit(Hooks.GetListenersFromRuntime());
            this.EmitSignal(SignalName.current_wave_changed, this._currentWave);
        }
    }

    public int total_levels { get; set; }
    public int total_waves { get; set; }

    public bool is_last_wave()
    {
        return this.current_wave >= this.total_waves;
    }
}
