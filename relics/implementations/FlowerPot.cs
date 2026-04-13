public sealed class FlowerPot : Relic
{
    private const int _healthBonus = 1;

    public FlowerPot() : base("flower_pot")
    {
    }

    public override void OnWaveFinished()
    {
        RunContextRuntime.Status.Heal(_healthBonus);
    }
}
