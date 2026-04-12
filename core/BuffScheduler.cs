using Godot;

[GlobalClass]
public partial class BuffScheduler : RefCounted
{
    [Signal]
    public delegate void buff_expiredEventHandler(Variant buff);

    [Signal]
    public delegate void buff_appliedEventHandler(Variant buff);

    private GodotObject progress;

    public BuffScheduler()
    {
    }

    public BuffScheduler(Variant progress_p)
    {
        this.progress = progress_p.AsGodotObject();
    }

    public void schedule(Variant buffVar)
    {
        GodotObject buff = buffVar.AsGodotObject();
        GodotObject duration = buff?.Get("duration").AsGodotObject();
        if (buff == null || duration == null)
        {
            return;
        }

        float seconds = duration.Get("seconds_duration").AsSingle();
        int waves = duration.Get("waves_duration").AsInt32();

        if (seconds > 0)
        {
            _ = this._schedule_in_seconds(buff, seconds);
        }
        else if (waves > 0)
        {
            _ = this._schedule_in_waves(buff, waves);
        }
    }

    private async System.Threading.Tasks.Task _schedule_in_seconds(GodotObject buff, float seconds)
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return;
        }

        await ToSignal(tree.CreateTimer(seconds, false), Timer.SignalName.Timeout);
        this._remove_buff(buff);
    }

    private async System.Threading.Tasks.Task _schedule_in_waves(GodotObject buff, int waves)
    {
        if (this.progress == null)
        {
            return;
        }

        int targetWave = this.progress.Get("current_wave").AsInt32() + waves;
        while (this.progress.Get("current_wave").AsInt32() < targetWave)
        {
            await ToSignal(this.progress, "current_wave_finished");
        }

        this._remove_buff(buff);
    }

    private void _remove_buff(GodotObject buff)
    {
        EmitSignal(SignalName.buff_expired, buff);

        Variant residualVar = buff.Get("residual_buff");
        GodotObject residual = residualVar.AsGodotObject();
        if (residual != null)
        {
            this._add_residual(residual);
        }
    }

    private void _add_residual(GodotObject buff)
    {
        EmitSignal(SignalName.buff_applied, buff);

        GodotObject duration = buff.Get("duration").AsGodotObject();
        if (duration != null)
        {
            this.schedule(buff);
        }
    }
}
