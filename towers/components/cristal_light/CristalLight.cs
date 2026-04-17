using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class CristalLight : PointLight2D
{
    [Export] public float attack_animation_time = 0.5f;
    [Export] public float max_scale = 2f;
    [Export] public float min_scale = 0.01f;
    [Export] public float max_energy = 2.0f;
    [Export] public float min_energy = 0f;

    private Tween _attackScaleTween;
    private Tween _attackEnergyTween;

    public async void play()
    {
        await turn_on();
        await turn_off();
    }

    public async Task turn_on()
    {
        KillPreviousAnimation();
        _attackEnergyTween = CreateTween();
        _attackScaleTween = CreateTween();
        _attackEnergyTween.TweenProperty(this, "energy", max_energy, attack_animation_time);
        _attackScaleTween.TweenProperty(this, "texture_scale", max_scale, attack_animation_time);
        await ToSignal(_attackEnergyTween, Tween.SignalName.Finished);
    }

    public async Task turn_off()
    {
        KillPreviousAnimation();
        _attackEnergyTween = CreateTween();
        _attackScaleTween = CreateTween();
        _attackEnergyTween.TweenProperty(this, "energy", min_energy, attack_animation_time);
        _attackScaleTween.TweenProperty(this, "texture_scale", min_scale, attack_animation_time);
        await ToSignal(_attackEnergyTween, Tween.SignalName.Finished);
    }

    private void KillPreviousAnimation()
    {
        if (_attackScaleTween != null && _attackScaleTween.IsRunning())
        {
            _attackScaleTween.Kill();
        }

        if (_attackEnergyTween != null && _attackEnergyTween.IsRunning())
        {
            _attackEnergyTween.Kill();
        }
    }
}
