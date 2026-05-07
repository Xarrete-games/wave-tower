using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class DebuffHandler : Node
{
    public static readonly Color BurnColor = Colors.DarkOrange;
    public static readonly Color FrostColor = Colors.Aqua;
    public static readonly Color DefaultColor = Colors.White;

    public List<EnemyDebuffInstance> Debuffs = new();

    public void AddDebuff(EnemyDebuff debuff, int amount, Enemy enemy)
    {
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

    public void UpdateAll(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        float now = Time.GetTicksMsec() / 1000.0f;

        for (int i = Debuffs.Count - 1; i >= 0; i--)
        {
            EnemyDebuffInstance inst = Debuffs[i];
            EnemyDebuff debuff = inst.Debuff;

            if (debuff.TickInterval > 0.0f && now >= inst.NextTickTime)
            {
                debuff.OnTick(enemy);
                inst.NextTickTime += debuff.TickInterval;
            }

            if (now >= inst.ExpireTime)
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
            if ((int)Debuffs[i].Debuff.DebuffType == debuffType)
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
            result.Add(Debuffs[i].Debuff);
        }

        return result;
    }
}

