using Godot;

public class Duration
{
    public float seconds_duration = 0.0f;
    public int waves_duration = 0;

    public Duration(float p_seconds_duration = 0.0f, int p_waves_duration = 0)
    {
        this.seconds_duration = p_seconds_duration;
        this.waves_duration = p_waves_duration;
    }
}
