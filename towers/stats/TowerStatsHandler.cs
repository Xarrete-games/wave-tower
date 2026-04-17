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
        get => this._stats_acc;
        set
        {
            this._stats_acc = value;
            this._update_stats();
        }
    }

    public TowerStats stats;
    public BuffScheduler buff_scheduler;

    public override void _Ready()
    {
        RunContext runContext = GetNodeOrNull<RunContext>("/root/RunContext");
        this.buff_scheduler = new BuffScheduler(runContext?.progress);
        this.buff_scheduler.buff_expired += this._on_scheduled_buff_expired;
        this.buff_scheduler.buff_applied += this._on_scheduled_buff_applied;
    }

    public void set_data(TowerData stats_configuration, int _p_tower_type)
    {
        this.base_stats = stats_configuration?.stats?.duplicate() ?? new TowerStats();
        this.stats_on_level = stats_configuration?.stats_on_level?.duplicate() ?? new TowerStats();
        this.stats = this.base_stats.duplicate();
        this._update_stats();
    }

    public void add_buff(TowerBuff tower_buff)
    {
        this.buffs.Add(tower_buff);

        if (tower_buff?.duration != null)
        {
            this.buff_scheduler.schedule(tower_buff);
        }

        this._rebuild_stats_acc();
    }

    public void remove_buff(string source_id)
    {
        int initialSize = this.buffs.Count;
        for (int i = this.buffs.Count - 1; i >= 0; i--)
        {
            TowerBuff buffObj = this.buffs[i];
            string buffSourceId = buffObj?.source?.TypeId ?? string.Empty;
            if (buffSourceId == source_id)
            {
                this.buffs.RemoveAt(i);
            }
        }

        if (this.buffs.Count != initialSize)
        {
            this._rebuild_stats_acc();
        }
    }

    public void level_up(int _new_level)
    {
        this.base_stats.add_stats(this.stats_on_level);
        this._update_stats();
    }

    private void _on_scheduled_buff_applied(TowerBuff buff)
    {
        this.buff_applied?.Invoke(buff);
    }

    private void _on_scheduled_buff_expired(TowerBuff buff)
    {
        string sourceId = buff?.source?.TypeId ?? string.Empty;
        this.buff_expired?.Invoke(sourceId);
    }

    private void _rebuild_stats_acc()
    {
        TowerStatsAccumulator acc = new();
        for (int i = 0; i < this.buffs.Count; i++)
        {
            TowerBuffStatsModifier buff = this.buffs[i] as TowerBuffStatsModifier;
            buff?.contribute(acc);
        }

        this.stats_acc = acc;
    }

    private void _update_stats()
    {
        if (this.stats == null || this.base_stats == null)
        {
            return;
        }

        TowerStatsAccumulator total = this.stats_acc ?? new TowerStatsAccumulator();

        this.stats.damage = (this.base_stats.damage + total.flat_damage) * (1.0f + total.damage_mult);

        float attackRangeMultiplier = 1.0f + total.attack_range_mult;
        this.stats.attack_range = (this.base_stats.attack_range + total.flat_attack_range) * attackRangeMultiplier;

        float attackSpeedMultiplier = 1.0f + total.attack_speed_mult;
        this.stats.attack_speed = (this.base_stats.attack_speed + total.flat_attack_speed) * attackSpeedMultiplier;

        this.stats.critic_chance = (this.base_stats.critic_chance + total.flat_critic_chance) * (1.0f + total.critic_chance_mult);
        this.stats.critic_damage = (this.base_stats.critic_damage + total.flat_critic_damage) * (1.0f + total.critic_damage_mult);

        this.stats_change?.Invoke(this.stats);
    }
}
