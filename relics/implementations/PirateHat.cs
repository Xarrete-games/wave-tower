public sealed class PirateHat : RelicModel
{
    private const double _chanceToRecoverConsumable = 0.5;
    private readonly System.Random _random = new();

    public PirateHat() : base("pirate_hat")
    {
    }

    public override void OnConsumableUsed(ConsumableModel consumable)
    {
        double value = this._random.NextDouble();
        if (value < _chanceToRecoverConsumable)
        {
            RunContextRuntime.ConsumablesManager.AddConsumable(consumable);
        }
    }
}
