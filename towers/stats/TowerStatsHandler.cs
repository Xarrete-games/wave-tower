using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class TowerStatsHandler : Node
{
    public event Action<TowerStats> stats_change;
    public event Action<TowerBuff> buff_applied;
    public event Action<string> buff_expired;

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
    public BuffScheduler buff_scheduler;

    public override void _Ready()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        buff_scheduler = new BuffScheduler(runContext?.progress);
        buff_scheduler.buff_expired += OnScheduledBuffExpired;
        buff_scheduler.buff_applied += OnScheduledBuffApplied;
    }

    public void set_data(TowerData stats_configuration, int _p_tower_type)
    {
        base_stats = stats_configuration?.stats?.duplicate() ?? new TowerStats();
        stats_on_level = stats_configuration?.stats_on_level?.duplicate() ?? new TowerStats();
        stats = base_stats.duplicate();
        UpdateStats();
    }

    public void add_buff(TowerBuff tower_buff)
    {
        buffs.Add(tower_buff);

        if (tower_buff?.duration != null)
        {
            buff_scheduler.schedule(tower_buff);
        }

        RebuildStatsAcc();
    }

    public void remove_buff(string source_id)
    {
        int initialSize = buffs.Count;
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buffObj = buffs[i];
            string buffSourceId = buffObj?.source?.TypeId ?? string.Empty;
            if (buffSourceId == source_id)
            {
                buffs.RemoveAt(i);
            }
        }

        if (buffs.Count != initialSize)
        {
            RebuildStatsAcc();
        }
    }

    public void level_up(int _new_level)
    {
        base_stats.add_stats(stats_on_level);
        UpdateStats();
    }

    private void OnScheduledBuffApplied(TowerBuff buff)
    {
        buff_applied?.Invoke(buff);
    }

    private void OnScheduledBuffExpired(TowerBuff buff)
    {
        string sourceId = buff?.source?.TypeId ?? string.Empty;
        buff_expired?.Invoke(sourceId);
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

        stats_change?.Invoke(stats);
    }
}
