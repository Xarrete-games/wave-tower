using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class DebuffHandler : Node
{
    public static readonly Color BURN_COLOR = Colors.DarkOrange;
    public static readonly Color FROST_COLOR = Colors.Aqua;
    public static readonly Color DEFAULT_COLOR = Colors.White;

    public List<EnemyDebuffInstance> Debuffs = new();

    public void AddDebuff(EnemyDebuff debuff, int amount, Variant enemyVar)
    {
        Enemy enemy = enemyVar.As<Enemy>();
        if (debuff == null || enemy == null)
        {
            return;
        }

        EnemyDebuffModel.DebuffType debuffType = debuff.DebuffType == EnemyDebuff.Type.Burn
            ? EnemyDebuffModel.DebuffType.Burn
            : EnemyDebuffModel.DebuffType.Frost;

        var debuffModel = new EnemyDebuffModel
        {
            Id = debuff.Data?.Id ?? string.Empty,
            Type = debuffType,
            Value = debuff.Value,
            Duration = debuff.Duration,
            TickInterval = debuff.TickInterval,
            MaxStacks = debuff.MaxStacks,
        };

        DebuffContext ctx = new(debuffModel, amount);
        Hooks.OnDebuffApplied(Hooks.GetListenersFromRuntime(), ctx, enemy);
        int stacks = ctx.Stacks;

        for (int i = 0; i < stacks; i++)
        {
            if (GetStacks((int)debuff.DebuffType) >= debuff.MaxStacks)
            {
                break;
            }

            EnemyDebuffInstance instance = new(debuff);
            Debuffs.Add(instance);
            enemy.HealthBar?.SetDebuffs(Debuffs);
            debuff.OnApply(enemy);
        }
    }

    public void UpdateAll(Variant enemyVar)
    {
        Enemy enemy = enemyVar.As<Enemy>();
        if (enemy == null)
        {
            return;
        }

        float now = Time.GetTicksMsec() / 1000.0f;

        for (int i = Debuffs.Count - 1; i >= 0; i--)
        {
            EnemyDebuffInstance inst = Debuffs[i];
            EnemyDebuff debuff = inst.debuff;

            if (debuff.TickInterval > 0.0f && now >= inst.next_tick_time)
            {
                debuff.OnTick(enemy);
                inst.next_tick_time += debuff.TickInterval;
            }

            if (now >= inst.expire_time)
            {
                debuff.OnExpire(enemy);
                Debuffs.RemoveAt(i);
                enemy.HealthBar?.SetDebuffs(Debuffs);
            }
        }
    }

    public int GetStacks(int debuffType)
    {
        int count = 0;
        for (int i = 0; i < Debuffs.Count; i++)
        {
            if ((int)Debuffs[i].debuff.DebuffType == debuffType)
            {
                count += 1;
            }
        }

        return count;
    }

    public bool HasAnyDebuff()
    {
        return Debuffs.Count > 0;
    }

    public List<EnemyDebuff> GetActiveDebuffs()
    {
        List<EnemyDebuff> result = new();
        for (int i = 0; i < Debuffs.Count; i++)
        {
            result.Add(Debuffs[i].debuff);
        }

        return result;
    }
}

