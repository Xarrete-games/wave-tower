using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class CristalLight : PointLight2D
{
    [Export] public float AttackAnimationTime = 0.5f;
    [Export] public float MaxScale = 2f;
    [Export] public float MinScale = 0.01f;
    [Export] public float MaxEnergy = 2.0f;
    [Export] public float MinEnergy = 0f;

    private Tween _attackScaleTween;
    private Tween _attackEnergyTween;

    public void Play()
    {
        _ = PlayAsync();
    }

    private async Task PlayAsync()
    {
        await TurnOn();
        await TurnOff();
    }

    public async Task TurnOn()
    {
        KillPreviousAnimation();
        _attackEnergyTween = CreateTween();
        _attackScaleTween = CreateTween();
        _attackEnergyTween.TweenProperty(this, "energy", MaxEnergy, AttackAnimationTime);
        _attackScaleTween.TweenProperty(this, "texture_scale", MaxScale, AttackAnimationTime);
        await ToSignal(_attackEnergyTween, Tween.SignalName.Finished);
    }

    public async Task TurnOff()
    {
        KillPreviousAnimation();
        _attackEnergyTween = CreateTween();
        _attackScaleTween = CreateTween();
        _attackEnergyTween.TweenProperty(this, "energy", MinEnergy, AttackAnimationTime);
        _attackScaleTween.TweenProperty(this, "texture_scale", MinScale, AttackAnimationTime);
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
