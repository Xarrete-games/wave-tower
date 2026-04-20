using Godot;
public class EnemyDebuff : EnemyEffect
{
    public enum Type
    {
        Frost,
        Burn,
    }

    public Type DebuffType;
    public EnemyDebuffData Data;
    public Source Source;
    public float Value = 0.0f;
    public float Duration = 0.0f;
    public float TickInterval = 0.0f;
    public int MaxStacks = 99;

    public EnemyDebuff()
    {
    }

    public virtual void Init(EnemyDebuffData debuffData, Source debuffSource)
    {
        Data = debuffData;
        Source = debuffSource;
        DebuffType = (Type)debuffData.DebuffType;
        Value = debuffData.Value;
        Duration = debuffData.Duration;
        TickInterval = debuffData.TickInterval;
        MaxStacks = debuffData.MaxStacks;
    }

    public static EnemyDebuff CreateFrost(Source source)
    {
        EnemyDebuffData data = DataLoader.Instance?.GetDebuffData((int)Type.Frost);
        if (data == null)
        {
            return null;
        }

        EnemyDebuff debuff = data.CreateDebuff();
        debuff?.Init(data, source);
        return debuff;
    }

    public static EnemyDebuff CreateBurn(Source source)
    {
        EnemyDebuffData data = DataLoader.Instance?.GetDebuffData((int)Type.Burn);
        if (data == null)
        {
            return null;
        }

        EnemyDebuff debuff = data.CreateDebuff();
        debuff?.Init(data, source);
        return debuff;
    }

    public static EnemyDebuff CreateFromType(int type, Source source)
    {
        return type switch
        {
            (int)Type.Frost => CreateFrost(source),
            (int)Type.Burn => CreateBurn(source),
            _ => null,
        };
    }

    public virtual void OnApply(Enemy enemy)
    {
    }

    public virtual void OnTick(Enemy enemy)
    {
    }

    public virtual void OnExpire(Enemy enemy)
    {
    }
}


