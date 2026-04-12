using Godot;
using Godot.Collections;

[GlobalClass]
public partial class DebuffHandler : Node
{
    public static readonly Color BURN_COLOR = Colors.DarkOrange;
    public static readonly Color FROST_COLOR = Colors.Aqua;
    public static readonly Color DEFAULT_COLOR = Colors.White;

    public Array<EnemyDebuffInstance> debuffs = new();

    public void add_debuff(EnemyDebuff debuff, int amount, Variant enemyVar)
    {
        GodotObject enemy = enemyVar.AsGodotObject();
        if (debuff == null || enemy == null)
        {
            return;
        }

        DebuffContext ctx = new(debuff, amount);
        Hooks.on_debuff_applied(ctx, enemy);
        int stacks = ctx.stacks;

        for (int i = 0; i < stacks; i++)
        {
            if (this.get_stacks((int)debuff.type) >= debuff.max_stacks)
            {
                break;
            }

            EnemyDebuffInstance instance = new(debuff);
            this.debuffs.Add(instance);
            enemy.Get("health_bar").AsGodotObject()?.Call("set_debuffs", this.debuffs);
            debuff.on_apply(enemy);
        }
    }

    public void update_all(Variant enemyVar)
    {
        GodotObject enemy = enemyVar.AsGodotObject();
        if (enemy == null)
        {
            return;
        }

        float now = Time.GetTicksMsec() / 1000.0f;

        for (int i = this.debuffs.Count - 1; i >= 0; i--)
        {
            EnemyDebuffInstance inst = this.debuffs[i];
            EnemyDebuff debuff = inst.debuff;

            if (debuff.tick_interval > 0.0f && now >= inst.next_tick_time)
            {
                debuff.on_tick(enemy);
                inst.next_tick_time += debuff.tick_interval;
            }

            if (now >= inst.expire_time)
            {
                debuff.on_expire(enemy);
                this.debuffs.RemoveAt(i);
                enemy.Get("health_bar").AsGodotObject()?.Call("set_debuffs", this.debuffs);
            }
        }
    }

    public int get_stacks(int debuff_type)
    {
        int count = 0;
        for (int i = 0; i < this.debuffs.Count; i++)
        {
            if ((int)this.debuffs[i].debuff.type == debuff_type)
            {
                count += 1;
            }
        }

        return count;
    }

    public bool has_any_defbuff()
    {
        return this.debuffs.Count > 0;
    }

    public Array<EnemyDebuff> get_active_debuffs()
    {
        Array<EnemyDebuff> result = new();
        for (int i = 0; i < this.debuffs.Count; i++)
        {
            result.Add(this.debuffs[i].debuff);
        }

        return result;
    }
}
