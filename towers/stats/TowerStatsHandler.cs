using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class TowerStatsHandler : Node
{
    public event Action<TowerStats> StatsChanged;
    public event Action<TowerBuff> BuffApplied;
    public event Action<string> BuffExpired;

    public TowerStats BaseStats;
    public TowerStats StatsOnLevel;
    public List<TowerBuff> Buffs = new();

    private TowerStatsAccumulator _statsAcc = new();
    public TowerStatsAccumulator StatsAcc
    {
        get => _statsAcc;
        set
        {
            _statsAcc = value;
            UpdateStats();
        }
    }

    public TowerStats Stats;
    public BuffScheduler BuffScheduler;

    public override void _Ready()
    {
        RunContext runContext = RunContext.Instance;
        BuffScheduler = new BuffScheduler(runContext.Progress);
        BuffScheduler.BuffExpired += OnScheduledBuffExpired;
        BuffScheduler.BuffApplied += OnScheduledBuffApplied;
    }

    public void SetData(TowerData statsConfiguration, int towerType)
    {
        BaseStats = statsConfiguration?.Stats?.Duplicate() ?? new TowerStats();
        StatsOnLevel = statsConfiguration?.StatsOnLevel?.Duplicate() ?? new TowerStats();
        Stats = BaseStats.Duplicate();
        UpdateStats();
    }

    public void AddBuff(TowerBuff towerBuff)
    {
        Buffs.Add(towerBuff);

        if (towerBuff?.Duration != null)
        {
            BuffScheduler.Schedule(towerBuff);
        }

        RebuildStatsAcc();
    }

    public void RemoveBuff(string sourceId)
    {
        int initialSize = Buffs.Count;
        for (int i = Buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buffObj = Buffs[i];
            string buffSourceId = buffObj?.Source?.TypeId ?? string.Empty;
            if (buffSourceId == sourceId)
            {
                Buffs.RemoveAt(i);
            }
        }

        if (Buffs.Count != initialSize)
        {
            RebuildStatsAcc();
        }
    }

    public void LevelUp(int newLevel)
    {
        BaseStats.AddStats(StatsOnLevel);
        UpdateStats();
    }

    private void OnScheduledBuffApplied(TowerBuff buff)
    {
        BuffApplied?.Invoke(buff);
    }

    private void OnScheduledBuffExpired(TowerBuff buff)
    {
        string sourceId = buff?.Source?.TypeId ?? string.Empty;
        BuffExpired?.Invoke(sourceId);
    }

    private void RebuildStatsAcc()
    {
        TowerStatsAccumulator acc = new();
        for (int i = 0; i < Buffs.Count; i++)
        {
            TowerBuffStatsModifier buff = Buffs[i] as TowerBuffStatsModifier;
            buff?.Contribute(acc);
        }

        StatsAcc = acc;
    }

    private void UpdateStats()
    {
        if (Stats == null || BaseStats == null)
        {
            return;
        }

        TowerStatsAccumulator total = StatsAcc ?? new TowerStatsAccumulator();

        Stats.Damage = (BaseStats.Damage + total.FlatDamage) * (1.0f + total.DamageMult);

        float attackRangeMultiplier = 1.0f + total.AttackRangeMult;
        Stats.AttackRange = (BaseStats.AttackRange + total.FlatAttackRange) * attackRangeMultiplier;

        float attackSpeedMultiplier = 1.0f + total.AttackSpeedMult;
        Stats.AttackSpeed = (BaseStats.AttackSpeed + total.FlatAttackSpeed) * attackSpeedMultiplier;

        Stats.CritChance = (BaseStats.CritChance + total.FlatCritChance) * (1.0f + total.CritChanceMult);
        Stats.CritDamage = (BaseStats.CritDamage + total.FlatCritDamage) * (1.0f + total.CritDamageMult);

        StatsChanged?.Invoke(Stats);
    }
}
