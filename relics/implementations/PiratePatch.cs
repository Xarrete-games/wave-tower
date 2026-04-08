public sealed class PiratePatch : RelicModel
{
    private const int _healthAmount = 2;

    public PiratePatch() : base("pirate_patch")
    {
    }

    public override void OnConsumableUsed(ConsumableModel consumable)
    {
        RunContextRuntime.Status.Heal(_healthAmount);
    }
}
