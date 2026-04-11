using Godot;

public partial class SliderVolumen : HSlider
{
    [Export]
    public string bus_name = string.Empty;

    private int _busIndex;

    public override void _Ready()
    {
        this._busIndex = AudioServer.GetBusIndex(this.bus_name);
        this.ValueChanged += this._on_value_changed;
        this.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(this._busIndex));
    }

    private void _on_value_changed(double newValue)
    {
        AudioServer.SetBusVolumeDb(this._busIndex, Mathf.LinearToDb((float)newValue));
    }

    private void _on_button_pressed()
    {
        GetParent()?.GetParent()?.GetParent()?.GetParent()?.GetParent()?.QueueFree();
        GetTree().Paused = !GetTree().Paused;
    }
}
