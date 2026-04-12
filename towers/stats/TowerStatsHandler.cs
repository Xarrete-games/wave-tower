using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TowerStatsHandler : Node
{
    [Signal]
    public delegate void stats_changeEventHandler(Variant new_stats);

    [Signal]
    public delegate void buff_appliedEventHandler(Variant buff);

    [Signal]
    public delegate void buff_expiredEventHandler(string source_id);

    public TowerStats base_stats;
    public TowerStats stats_on_level;
    public Array<Variant> buffs = new();

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
        this.buff_scheduler.Connect("buff_expired", Callable.From<Variant>(this._on_scheduled_buff_expired));
        this.buff_scheduler.Connect("buff_applied", Callable.From<Variant>(this._on_scheduled_buff_applied));
    }

    public void set_data(Variant stats_configuration, Variant _p_tower_type)
    {
        GodotObject config = stats_configuration.AsGodotObject();
        this.base_stats = config?.Get("stats").As<TowerStats>()?.duplicate() ?? new TowerStats();
        this.stats_on_level = config?.Get("stats_on_level").As<TowerStats>()?.duplicate() ?? new TowerStats();
        this.stats = this.base_stats.duplicate();
        this._update_stats();
    }

    public void add_buff(Variant tower_buff)
    {
        this.buffs.Add(tower_buff);

        GodotObject buffObj = tower_buff.AsGodotObject();
        if (buffObj?.Get("duration").AsGodotObject() != null)
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
            GodotObject buffObj = this.buffs[i].AsGodotObject();
            string buffSourceId = buffObj?.Get("source").AsGodotObject()?.Get("type_id").AsString() ?? string.Empty;
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

    private void _on_scheduled_buff_applied(Variant buff)
    {
        EmitSignal(SignalName.buff_applied, buff);
    }

    private void _on_scheduled_buff_expired(Variant buff)
    {
        GodotObject buffObj = buff.AsGodotObject();
        string sourceId = buffObj?.Get("source").AsGodotObject()?.Get("type_id").AsString() ?? string.Empty;
        EmitSignal(SignalName.buff_expired, sourceId);
    }

    private void _rebuild_stats_acc()
    {
        TowerStatsAccumulator acc = new();
        for (int i = 0; i < this.buffs.Count; i++)
        {
            TowerBuffStatsModifier buff = this.buffs[i].As<TowerBuffStatsModifier>();
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

        EmitSignal(SignalName.stats_change, this.stats);
    }
}
