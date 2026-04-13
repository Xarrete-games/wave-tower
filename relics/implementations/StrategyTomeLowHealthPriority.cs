using System.Collections.Generic;

public sealed class StrategyTomeLowHealthPriority : Relic
{
    public StrategyTomeLowHealthPriority() : base("strategy_tome_low_health_priority")
    {
    }

    public override void OnGetTargetingModes(List<TowerTargetingMode> targetingModes)
    {
        targetingModes.Add(TowerTargetingMode.LowHp);
    }
}
