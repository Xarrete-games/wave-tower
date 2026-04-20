using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class TowerStatsHandler : Node
{
    public event Action<TowerStats> StatsChanged;
    public event Action<TowerBuff> BuffApplied;
    public event Action<string> BuffExpired;

    public TowerStats base_stats;
    public TowerStats stats_on_level;
    public List<TowerBuff> buffs = new();

    private TowerStatsAccumulator _stats_acc = new();
    public TowerStatsAccumulator stats_acc
    {
        get => _stats_acc;
        set
        {
            _stats_acc = value;
            UpdateStats();
        }
    }

    public TowerStats stats;
    public BuffScheduler BuffScheduler;

    public override void _Ready()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        BuffScheduler = new BuffScheduler(runContext?.Progress);
        BuffScheduler.BuffExpired += OnScheduledBuffExpired;
        BuffScheduler.BuffApplied += OnScheduledBuffApplied;
    }

    public void SetData(TowerData statsConfiguration, int towerType)
    {
        base_stats = statsConfiguration?.stats?.duplicate() ?? new TowerStats();
        stats_on_level = statsConfiguration?.stats_on_level?.duplicate() ?? new TowerStats();
        stats = base_stats.duplicate();
        UpdateStats();
    }

    public void AddBuff(TowerBuff towerBuff)
    {
        buffs.Add(towerBuff);

        if (towerBuff?.duration != null)
        {
            BuffScheduler.Schedule(towerBuff);
        }

        RebuildStatsAcc();
    }

    public void RemoveBuff(string sourceId)
    {
        int initialSize = buffs.Count;
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buffObj = buffs[i];
            string buffSourceId = buffObj?.source?.TypeId ?? string.Empty;
            if (buffSourceId == sourceId)
            {
                buffs.RemoveAt(i);
            }
        }

        if (buffs.Count != initialSize)
        {
            RebuildStatsAcc();
        }
    }

    public void LevelUp(int newLevel)
    {
        base_stats.add_stats(stats_on_level);
        UpdateStats();
    }

    private void OnScheduledBuffApplied(TowerBuff buff)
    {
        BuffApplied?.Invoke(buff);
    }

    private void OnScheduledBuffExpired(TowerBuff buff)
    {
        string sourceId = buff?.source?.TypeId ?? string.Empty;
        BuffExpired?.Invoke(sourceId);
    }

    private void RebuildStatsAcc()
    {
        TowerStatsAccumulator acc = new();
        for (int i = 0; i < buffs.Count; i++)
        {
            TowerBuffStatsModifier buff = buffs[i] as TowerBuffStatsModifier;
            buff?.contribute(acc);
        }

        stats_acc = acc;
    }

    private void UpdateStats()
    {
        if (stats == null || base_stats == null)
        {
            return;
        }

        TowerStatsAccumulator total = stats_acc ?? new TowerStatsAccumulator();

        stats.damage = (base_stats.damage + total.flat_damage) * (1.0f + total.damage_mult);

        float attackRangeMultiplier = 1.0f + total.attack_range_mult;
        stats.attack_range = (base_stats.attack_range + total.flat_attack_range) * attackRangeMultiplier;

        float attackSpeedMultiplier = 1.0f + total.attack_speed_mult;
        stats.attack_speed = (base_stats.attack_speed + total.flat_attack_speed) * attackSpeedMultiplier;

        stats.critic_chance = (base_stats.critic_chance + total.flat_critic_chance) * (1.0f + total.critic_chance_mult);
        stats.critic_damage = (base_stats.critic_damage + total.flat_critic_damage) * (1.0f + total.critic_damage_mult);

        StatsChanged?.Invoke(stats);
    }
}
