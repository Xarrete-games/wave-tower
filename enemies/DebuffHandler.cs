using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class DebuffHandler : Node
{
    public static readonly Color BURN_COLOR = Colors.DarkOrange;
    public static readonly Color FROST_COLOR = Colors.Aqua;
    public static readonly Color DEFAULT_COLOR = Colors.White;

    public List<EnemyDebuffInstance> debuffs = new();

    public void add_debuff(EnemyDebuff debuff, int amount, Variant enemyVar)
    {
        Enemy enemy = enemyVar.As<Enemy>();
        if (debuff == null || enemy == null)
        {
            return;
        }

        EnemyDebuffModel.DebuffType debuffType = debuff.type == EnemyDebuff.Type.BURN
            ? EnemyDebuffModel.DebuffType.Burn
            : EnemyDebuffModel.DebuffType.Frost;

        var debuffModel = new EnemyDebuffModel
        {
            Id = debuff.data?.Id ?? string.Empty,
            Type = debuffType,
            Value = debuff.value,
            Duration = debuff.duration,
            TickInterval = debuff.TickInterval,
            MaxStacks = debuff.MaxStacks,
        };

        var enemyModel = new EnemyModel
        {
            MaxHealth = enemy.MaxHealth,
            RemainingHealth = enemy.health,
            ProgressRatio = enemy.get_progress_ratio(),
            GoldValue = enemy.gold_value,
            HasAnyDebuff = enemy.has_any_debuff(),
        };

        DebuffContext ctx = new(debuffModel, amount);
        Hooks.OnDebuffApplied(Hooks.GetListenersFromRuntime(), ctx, enemyModel);
        int stacks = ctx.Stacks;

        for (int i = 0; i < stacks; i++)
        {
            if (get_stacks((int)debuff.type) >= debuff.MaxStacks)
            {
                break;
            }

            EnemyDebuffInstance instance = new(debuff);
            debuffs.Add(instance);
            enemy.health_bar?.set_debuffs(debuffs);
            debuff.on_apply(enemy);
        }
    }

    public void update_all(Variant enemyVar)
    {
        Enemy enemy = enemyVar.As<Enemy>();
        if (enemy == null)
        {
            return;
        }

        float now = Time.GetTicksMsec() / 1000.0f;

        for (int i = debuffs.Count - 1; i >= 0; i--)
        {
            EnemyDebuffInstance inst = debuffs[i];
            EnemyDebuff debuff = inst.debuff;

            if (debuff.TickInterval > 0.0f && now >= inst.next_tick_time)
            {
                debuff.on_tick(enemy);
                inst.next_tick_time += debuff.TickInterval;
            }

            if (now >= inst.expire_time)
            {
                debuff.on_expire(enemy);
                debuffs.RemoveAt(i);
                enemy.health_bar?.set_debuffs(debuffs);
            }
        }
    }

    public int get_stacks(int DebuffType)
    {
        int count = 0;
        for (int i = 0; i < debuffs.Count; i++)
        {
            if ((int)debuffs[i].debuff.type == DebuffType)
            {
                count += 1;
            }
        }

        return count;
    }

    public bool has_any_defbuff()
    {
        return debuffs.Count > 0;
    }

    public List<EnemyDebuff> get_active_debuffs()
    {
        List<EnemyDebuff> result = new();
        for (int i = 0; i < debuffs.Count; i++)
        {
            result.Add(debuffs[i].debuff);
        }

        return result;
    }
}

