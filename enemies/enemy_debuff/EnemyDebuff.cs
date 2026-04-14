using Godot;

public class EnemyDebuff : EnemyEffect
{
    public enum Type
    {
        FROST,
        BURN,
    }

    public Type type;
    public EnemyDebuffData data;
    public Source source;
    public float value = 0.0f;
    public float duration = 0.0f;
    public float tick_interval = 0.0f;
    public int max_stacks = 99;

    public EnemyDebuff()
    {
    }

    public virtual void init(EnemyDebuffData p_data, Source p_source)
    {
        this.data = p_data;
        this.source = p_source;
        this.type = (Type)p_data.debuff_type;
        this.value = p_data.value;
        this.duration = p_data.duration;
        this.tick_interval = p_data.tick_interval;
        this.max_stacks = p_data.max_stacks;
    }

    public static EnemyDebuff create_frost(Source p_source)
    {
        EnemyDebuffData p_data = DataLoader.Instance?.get_debuff_data((int)Type.FROST).As<EnemyDebuffData>();
        if (p_data == null)
        {
            return null;
        }

        EnemyDebuff debuff = p_data.create_debuff();
        debuff?.init(p_data, p_source);
        return debuff;
    }

    public static EnemyDebuff create_burn(Source p_source)
    {
        EnemyDebuffData p_data = DataLoader.Instance?.get_debuff_data((int)Type.BURN).As<EnemyDebuffData>();
        if (p_data == null)
        {
            return null;
        }

        EnemyDebuff debuff = p_data.create_debuff();
        debuff?.init(p_data, p_source);
        return debuff;
    }

    public static EnemyDebuff create_from_type(int p_type, Source p_source)
    {
        return p_type switch
        {
            (int)Type.FROST => create_frost(p_source),
            (int)Type.BURN => create_burn(p_source),
            _ => null,
        };
    }

    public virtual void on_apply(Enemy enemy)
    {
    }

    public virtual void on_tick(Enemy enemy)
    {
    }

    public virtual void on_expire(Enemy enemy)
    {
    }
}
