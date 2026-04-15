
using System;
using System.Collections.Generic;
using Godot;


public static class Hooks
{
    public static void on_before_attack(AttackContext ctx)
    {
        // Legacy Variant dispatch cannot carry pure C# contexts.
    }

    public static void on_before_damage(DamageContext ctx)
    {
        // Legacy Variant dispatch cannot carry pure C# contexts.
    }

    public static void on_debuff_applied(DebuffContext ctx, Variant target)
    {
        // Legacy Variant dispatch cannot carry pure C# contexts.
    }

    public static void on_enemy_die(Variant enemy, Variant attack)
    {
        // Legacy Variant bridge removed from gameplay path. Prefer OnEnemyDie with typed models.
    }

    public static void on_wave_init()
    {
        OnWaveInit(GetListenersFromRuntime());
    }

    public static void on_wave_finished()
    {
        OnWaveFinished(GetListenersFromRuntime());
    }

    public static void on_get_price(PriceContext ctx)
    {
        ctx.FinalPrice = (int)MathF.Round(ctx.BasePrice * (1f - ctx.Discount));
    }

    public static void on_tower_placed(Tower tower)
    {
        if (tower == null)
        {
            return;
        }

        if (!RunContextRuntime.TowersManager.TryGetTowerByInstanceId(tower.GetInstanceId(), out TowerModel towerModel))
        {
            return;
        }

        OnTowerPlaced(GetListenersFromRuntime(), towerModel);
    }

    public static void on_get_targeting_modes(Godot.Collections.Array targetingModes)
    {
        if (targetingModes == null)
        {
            return;
        }

        var typedModes = new List<TowerTargetingMode>();
        for (int index = 0; index < targetingModes.Count; index++)
        {
            typedModes.Add((TowerTargetingMode)targetingModes[index].AsInt32());
        }

        OnGetTargetingModes(GetListenersFromRuntime(), typedModes);

        targetingModes.Clear();
        for (int index = 0; index < typedModes.Count; index++)
        {
            targetingModes.Add((int)typedModes[index]);
        }
    }

    public static void on_relic_added(Variant relicAdded)
    {
        Relic relic = relicAdded.Obj as Relic;
        if (relic == null)
        {
            return;
        }

        OnRelicAdded(GetListenersFromRuntime(), relic);
    }

    public static void on_consumable_used(Variant consumable)
    {
        // Legacy Variant bridge removed from gameplay path. Prefer OnConsumableUsed with typed models.
    }

    public static void on_before_get_loot(LootContext ctx)
    {
        // Legacy Variant dispatch cannot carry pure C# contexts.
    }

    public static void on_before_relic_reward(RelicsRewardsContext ctx)
    {
        // Legacy Variant dispatch cannot carry pure C# contexts.
    }

    public static void on_before_die(Status status)
    {
        if (status == null)
        {
            return;
        }

        var model = new StatusModel
        {
            MaxHealth = status.max_health,
            Health = status.health,
            Armor = status.armor,
        };

        OnBeforeDie(GetListenersFromRuntime(), model);
    }

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

    public static void OnDebuffApplied(List<AbstractModel> listeners, DebuffContext context, EnemyModel target)
    {
        ForEach(listeners, item => item.OnDebuffApplied(context, target));
    }

    public static void OnEnemyDie(List<AbstractModel> listeners, EnemyModel enemy, AttackModel attack)
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
