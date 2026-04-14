using Godot;
using System;
using System.Threading.Tasks;

public class BuffScheduler
{
    public event Action<TowerBuff> buff_expired;
    public event Action<TowerBuff> buff_applied;

    private RunProgress progress;

    public BuffScheduler()
    {
    }

    public BuffScheduler(RunProgress progress_p)
    {
        this.progress = progress_p;
    }

    public void schedule(TowerBuff buff)
    {
        Duration duration = buff?.duration;
        if (buff == null || duration == null)
        {
            return;
        }

        float seconds = duration.seconds_duration;
        int waves = duration.waves_duration;

        if (seconds > 0)
        {
            _ = this._schedule_in_seconds(buff, seconds);
        }
        else if (waves > 0)
        {
            _ = this._schedule_in_waves(buff, waves);
        }
    }

    private async Task _schedule_in_seconds(TowerBuff buff, float seconds)
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        await this.WaitForSecondsAsync(tree, seconds);
        this._remove_buff(buff);
    }

    private async Task _schedule_in_waves(TowerBuff buff, int waves)
    {
        if (this.progress == null)
        {
            return;
        }

        int targetWave = this.progress.current_wave + waves;
        while (this.progress.current_wave < targetWave)
        {
            await this.WaitForWaveFinishedAsync();
        }

        this._remove_buff(buff);
    }

    private Task WaitForWaveFinishedAsync()
    {
        if (this.progress == null)
        {
            return Task.CompletedTask;
        }

        TaskCompletionSource<bool> tcs = new();
        void Handler()
        {
            this.progress.current_wave_finished -= Handler;
            tcs.TrySetResult(true);
        }

        this.progress.current_wave_finished += Handler;
        return tcs.Task;
    }

    private Task WaitForSecondsAsync(SceneTree tree, float seconds)
    {
        if (tree == null)
        {
            return Task.CompletedTask;
        }

        TaskCompletionSource<bool> tcs = new();
        SceneTreeTimer timer = tree.CreateTimer(seconds, false);
        void Handler()
        {
            timer.Timeout -= Handler;
            tcs.TrySetResult(true);
        }

        timer.Timeout += Handler;
        return tcs.Task;
    }

    private void _remove_buff(TowerBuff buff)
    {
        this.buff_expired?.Invoke(buff);

        TowerBuff residual = buff?.residual_buff;
        if (residual != null)
        {
            this._add_residual(residual);
        }
    }

    private void _add_residual(TowerBuff buff)
    {
        this.buff_applied?.Invoke(buff);

        if (buff.duration != null)
        {
            this.schedule(buff);
        }
    }
}
