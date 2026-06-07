using System.Collections.Generic;

public sealed class Tower : AbstractModel
{
    public enum Type
    {
        FIRE,
        LIGHTNING,
        FROST,
    }

    public enum TargetingMode
    {
        FIRST_IN_PROGRESS,
        HIGH_HP,
        LOW_HP,
    }

    public const int MAX_LEVEL = 2;

    public Type TowerType { get; set; } = Type.FIRE;
    public int BuildPrice { get; set; }
    public int UpgradePrice { get; set; }
    public TowerStats Stats { get; set; }
    public TowerExpData ExpData { get; set; }
    public int CurrentTargetingMode { get; set; } = (int)TargetingMode.FIRST_IN_PROGRESS;
    public int Level { get; set; } = 1;
    public string Id { get; set; } = string.Empty;
    public string TypeId { get; set; } = string.Empty;
    public List<TowerBuff> Buffs { get; } = new();

    public Source DamageSource => new(Source.SourceType.TOWER, TypeId);

    public static string TargetingModeToString(int mode)
    {
        return mode switch
        {
            (int)TargetingMode.FIRST_IN_PROGRESS => "Progress",
            (int)TargetingMode.HIGH_HP => "High Health",
            (int)TargetingMode.LOW_HP => "Low Health",
            _ => "Unknown",
        };
    }

    public void AddBuff(TowerBuff towerBuff)
    {
        if (towerBuff == null)
        {
            return;
        }

        Buffs.Add(towerBuff);
    }

    public void RemoveBuffAt(int index)
    {
        if (index < 0 || index >= Buffs.Count)
        {
            return;
        }

        Buffs.RemoveAt(index);
    }

    public void Upgrade(Economy economy)
    {
        if (economy != null)
        {
            economy.Gold -= UpgradePrice;
        }

        Level += 1;
    }

    public bool IsMaxLevel()
    {
        return Level >= MAX_LEVEL;
    }

    public bool IsCriticalHit(float randomValue)
    {
        float critChance = Stats?.CritChance ?? 0f;
        return randomValue < critChance / 100f;
    }

    public TowerModel BuildTowerModel()
    {
        TowerModel.TowerType towerType = TowerType switch
        {
            Type.FIRE => TowerModel.TowerType.Fire,
            Type.LIGHTNING => TowerModel.TowerType.Lightning,
            Type.FROST => TowerModel.TowerType.Frost,
            _ => TowerModel.TowerType.Fire,
        };

        return new TowerModel
        {
            Id = Id,
            TypeId = TypeId,
            Type = towerType,
        };
    }

    public AttackModel BuildAttackModel(Attack attack)
    {
        var model = new AttackModel
        {
            Damage = attack?.Damage ?? 0f,
            CritChance = attack?.CritChance ?? 0f,
            IsCritical = attack?.IsCritical ?? false,
            IsExecution = attack?.IsExecution ?? false,
            Hits = attack?.Hits ?? 1,
            Bounces = attack?.Bounces ?? 0,
        };

        Source source = attack?.Source;
        model.Source = new SourceModel
        {
            Type = source?.Type switch
            {
                Source.SourceType.RELIC => SourceModel.SourceType.Relic,
                Source.SourceType.TOWER => SourceModel.SourceType.Tower,
                Source.SourceType.CONSUMABLE => SourceModel.SourceType.Consumable,
                Source.SourceType.DEBUFF => SourceModel.SourceType.Debuff,
                _ => SourceModel.SourceType.Global,
            },
            TypeId = source?.TypeId ?? string.Empty,
        };

        return model;
    }
}
