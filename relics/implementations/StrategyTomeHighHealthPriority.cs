using System.Collections.Generic;

public sealed class StrategyTomeHighHealthPriority : Relic
{
    public StrategyTomeHighHealthPriority() : base("strategy_tome_high_health_priority")
    {
    }

    public override void OnGetTargetingModes(List<TowerTargetingMode> targetingModes)
    {
        targetingModes.Add(TowerTargetingMode.HighHp);
    }
}
