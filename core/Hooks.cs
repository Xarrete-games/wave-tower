
using System;
using System.Collections.Generic;
using Godot;


public static class Hooks
{
    public static void on_before_attack(AttackContext ctx)
    {
        ForEachLegacyListener(item => item.Call("on_before_attack", ctx));
    }

    public static void on_before_damage(DamageContext ctx)
    {
        ForEachLegacyListener(item => item.Call("on_before_damage", ctx));
    }

    public static void on_debuff_applied(DebuffContext ctx, Variant target)
    {
        ForEachLegacyListener(item => item.Call("on_debuff_applied", ctx, target));
    }

    public static void on_enemy_die(Variant enemy, Variant attack)
    {
        ForEachLegacyListener(item => item.Call("on_enemy_die", enemy, attack));
    }

    public static void on_wave_init()
    {
        ForEachLegacyListener(item => item.Call("on_wave_init"));
    }

    public static void on_wave_finished()
    {
        ForEachLegacyListener(item => item.Call("on_wave_finished"));
    }

    public static void on_get_price(PriceContext ctx)
    {
        ForEachLegacyListener(item => item.Call("on_get_price", ctx));
        ctx.FinalPrice = (int)MathF.Round(ctx.BasePrice * (1f - ctx.Discount));
    }

    public static void on_tower_placed(Variant tower)
    {
        ForEachLegacyListener(item => item.Call("on_tower_placed", tower));
    }

    public static void on_get_targeting_modes(Godot.Collections.Array targetingModes)
    {
        ForEachLegacyListener(item => item.Call("on_get_targeting_modes", targetingModes));
    }

    public static void on_relic_added(Variant relicAdded)
    {
        ForEachLegacyListener(item => item.Call("on_relic_added", relicAdded));
    }

    public static void on_consumable_used(Variant consumable)
    {
        ForEachLegacyListener(item => item.Call("on_consumable_used", consumable));
    }

    public static void on_before_get_loot(LootContext ctx)
    {
        ForEachLegacyListener(item => item.Call("on_before_get_loot", ctx));
    }

    public static void on_before_relic_reward(RelicsRewardsContext ctx)
    {
        ForEachLegacyListener(item => item.Call("on_before_relic_reward", ctx));
    }

    public static void on_before_die(Status status)
    {
        ForEachLegacyListener(item => item.Call("on_before_die", status));
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

    private static void ForEachLegacyListener(Action<GodotObject> action)
    {
        if (action == null)
        {
            return;
        }

        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        RunContext runContext = tree?.Root?.GetNodeOrNull<RunContext>("/root/RunContext");
        if (runContext == null)
        {
            return;
        }

        // Relics are now pure C# runtime listeners; legacy dispatch only applies to tower listeners.

        Godot.Collections.Array<Variant> towerListeners = runContext.towers_manager?.get_tower_listeners();
        if (towerListeners != null)
        {
            for (int index = 0; index < towerListeners.Count; index++)
            {
                GodotObject item = towerListeners[index].AsGodotObject();
                if (item != null)
                {
                    action(item);
                }
            }
        }
    }
}
