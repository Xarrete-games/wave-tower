
using System;
using System.Collections.Generic;


public static class Hooks
{
    public static List<AbstractModel> GetListenersFromRuntime()
    {
        return RunContextRuntime.GetListeners();
    }

    public static List<AbstractModel> GetListeners(
        IEnumerable<AbstractModel> relicListeners,
        IEnumerable<AbstractModel> towerListeners)
    {
        var listeners = new List<AbstractModel>();

        if (relicListeners != null)
        {
            listeners.AddRange(relicListeners);
        }

        if (towerListeners != null)
        {
            listeners.AddRange(towerListeners);
        }

        return listeners;
    }

    public static void OnBeforeAttack(List<AbstractModel> listeners, AttackContext context)
    {
        ForEach(listeners, item => item.OnBeforeAttack(context));
    }

    public static void OnBeforeDamage(List<AbstractModel> listeners, DamageContext context)
    {
        ForEach(listeners, item => item.OnBeforeDamage(context));
    }

    public static void OnDebuffApplied(List<AbstractModel> listeners, DebuffContext context, Enemy target)
    {
        ForEach(listeners, item => item.OnDebuffApplied(context, target));
    }

    public static void OnEnemyDie(List<AbstractModel> listeners, Enemy enemy, AttackModel attack)
    {
        ForEach(listeners, item => item.OnEnemyDie(enemy, attack));
    }

    public static void OnWaveInit(List<AbstractModel> listeners)
    {
        ForEach(listeners, item => item.OnWaveInit());
    }

    public static void OnWaveFinished(List<AbstractModel> listeners)
    {
        ForEach(listeners, item => item.OnWaveFinished());
    }

    public static void OnGetPrice(List<AbstractModel> listeners, PriceContext context)
    {
        ForEach(listeners, item => item.OnGetPrice(context));
        context.FinalPrice = (int)MathF.Round(context.BasePrice * (1f - context.Discount));
    }

    public static void OnTowerPlaced(List<AbstractModel> listeners, TowerModel tower)
    {
        ForEach(listeners, item => item.OnTowerPlaced(tower));
    }

    public static void OnGetTargetingModes(List<AbstractModel> listeners, List<TowerTargetingMode> targetingModes)
    {
        ForEach(listeners, item => item.OnGetTargetingModes(targetingModes));
    }

    public static void OnRelicAdded(List<AbstractModel> listeners, Relic relicAdded)
    {
        ForEach(listeners, item => item.OnRelicAdded(relicAdded));
    }

    public static void OnConsumableUsed(List<AbstractModel> listeners, ConsumableModel consumable)
    {
        ForEach(listeners, item => item.OnConsumableUsed(consumable));
    }

    public static void OnBeforeGetLoot(List<AbstractModel> listeners, LootContext context)
    {
        ForEach(listeners, item => item.OnBeforeGetLoot(context));
    }

    public static void OnBeforeRelicReward(List<AbstractModel> listeners, RelicsRewardsContext context)
    {
        ForEach(listeners, item => item.OnBeforeRelicReward(context));
    }

    public static void OnBeforeDie(List<AbstractModel> listeners, StatusModel status)
    {
        ForEach(listeners, item => item.OnBeforeDie(status));
    }

    private static void ForEach(List<AbstractModel> listeners, Action<AbstractModel> action)
    {
        if (listeners == null || action == null)
        {
            return;
        }

        for (int index = 0; index < listeners.Count; index++)
        {
            AbstractModel item = listeners[index];
            if (item == null)
            {
                continue;
            }
            action(item);
        }
    }

}

