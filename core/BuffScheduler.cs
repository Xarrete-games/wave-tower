using Godot;
using System;
using System.Threading.Tasks;

public class BuffScheduler
{
    public event Action<TowerBuff> BuffExpired;
    public event Action<TowerBuff> BuffApplied;

    private RunProgress progress;

    public BuffScheduler()
    {
    }

    public BuffScheduler(RunProgress runProgress)
    {
        progress = runProgress;
    }

    public void Schedule(TowerBuff buff)
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
            _ = ScheduleInSecondsAsync(buff, seconds);
        }
        else if (waves > 0)
        {
            _ = ScheduleInWavesAsync(buff, waves);
        }
    }

    private async Task ScheduleInSecondsAsync(TowerBuff buff, float seconds)
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        await WaitForSecondsAsync(tree, seconds);
        RemoveBuff(buff);
    }

    private async Task ScheduleInWavesAsync(TowerBuff buff, int waves)
    {
        if (progress == null)
        {
            return;
        }

        int targetWave = progress.CurrentWave + waves;
        while (progress.CurrentWave < targetWave)
        {
            await WaitForWaveFinishedAsync();
        }

        RemoveBuff(buff);
    }

    private Task WaitForWaveFinishedAsync()
    {
        if (progress == null)
        {
            return Task.CompletedTask;
        }

        TaskCompletionSource<bool> tcs = new();
        void Handler()
        {
            progress.CurrentWaveFinished -= Handler;
            tcs.TrySetResult(true);
        }

        progress.CurrentWaveFinished += Handler;
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

    private void RemoveBuff(TowerBuff buff)
    {
        BuffExpired?.Invoke(buff);

        TowerBuff residual = buff?.residual_buff;
        if (residual != null)
        {
            AddResidual(residual);
        }
    }

    private void AddResidual(TowerBuff buff)
    {
        BuffApplied?.Invoke(buff);

        if (buff.duration != null)
        {
            Schedule(buff);
        }
    }
}
