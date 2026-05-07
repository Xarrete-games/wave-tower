public sealed class StrategyTomeEconomy : Relic
{
    public StrategyTomeEconomy() : base("strategy_tome_economy")
    {
    }

    public override void OnObtain()
    {
        RunContextRuntime.Economy.IsSellActive = true;
    }

    public override void OnRemove()
    {
        RunContextRuntime.Economy.IsSellActive = false;
    }
}
