using Godot;

public partial class SliderVolumen : HSlider
{
    [Export]
    public string BusName = string.Empty;

    private int _busIndex;

    public override void _Ready()
    {
        _busIndex = AudioServer.GetBusIndex(BusName);
        ValueChanged += OnValueChanged;
        Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(_busIndex));
    }

    private void OnValueChanged(double newValue)
    {
        AudioServer.SetBusVolumeDb(_busIndex, Mathf.LinearToDb((float)newValue));
    }

    private void OnButtonPressed()
    {
        GetParent()?.GetParent()?.GetParent()?.GetParent()?.GetParent()?.QueueFree();
        GetTree().Paused = !GetTree().Paused;
    }
}
