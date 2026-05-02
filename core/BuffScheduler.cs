using Godot;
using System;
using System.Threading.Tasks;

public class BuffScheduler
{
    public event Action<TowerBuff> BuffExpired;
    public event Action<TowerBuff> BuffApplied;

    private RunProgress _progress;

    public BuffScheduler()
    {
    }

    public BuffScheduler(RunProgress runProgress)
    {
        _progress = runProgress;
    }

    public void Schedule(TowerBuff buff)
    {
        Duration duration = buff?.Duration;
        if (buff == null || duration == null)
        {
            return;
        }

        float seconds = duration.SecondsDuration;
        int waves = duration.WavesDuration;

        if (seconds > 0)
        {
            AsyncTaskHelper.FireAndForget(ScheduleInSecondsAsync(buff, seconds), "BuffScheduler.ScheduleInSecondsAsync");
        }
        else if (waves > 0)
        {
            AsyncTaskHelper.FireAndForget(ScheduleInWavesAsync(buff, waves), "BuffScheduler.ScheduleInWavesAsync");
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
        if (_progress == null)
        {
            return;
        }

        int targetWave = _progress.CurrentWave + waves;
        while (_progress.CurrentWave < targetWave)
        {
            await WaitForWaveFinishedAsync();
        }

        RemoveBuff(buff);
    }

    private Task WaitForWaveFinishedAsync()
    {
        if (_progress == null)
        {
            return Task.CompletedTask;
        }

        TaskCompletionSource<bool> tcs = new();
        void Handler()
        {
            _progress.CurrentWaveFinished -= Handler;
            tcs.TrySetResult(true);
        }

        _progress.CurrentWaveFinished += Handler;
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

        TowerBuff residual = buff?.ResidualBuff;
        if (residual != null)
        {
            AddResidual(residual);
        }
    }

    private void AddResidual(TowerBuff buff)
    {
        BuffApplied?.Invoke(buff);

        if (buff.Duration != null)
        {
            Schedule(buff);
        }
    }
}
