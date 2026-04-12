using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class HealthBar : Control
{
    private static readonly PackedScene DEBUFF_SLOT = GD.Load<PackedScene>("uid://beixhmpysku3t");

    private const float MIN_HEALTH = 40.0f;
    private const float MAX_HEALTH = 5000.0f;
    private const float MIN_X_SIZE = 40.0f;
    private const float MAX_X_SIZE = 160.0f;

    [Export] public Control debuffs_conatiner;
    [Export] public TextureProgressBar texture_progress_bar;

    public System.Collections.Generic.Dictionary<int, DebuffSlot> debuffs_slots = new();
    public System.Collections.Generic.Dictionary<int, int> debuffs_count = new();
    public System.Collections.Generic.Dictionary<int, Variant> debuff_data_by_type = new();

    public void set_max_health(float value)
    {
        float clampedValue = Mathf.Clamp(value, MIN_HEALTH, MAX_HEALTH);
        float newXSize = Mathf.Remap(clampedValue, MIN_HEALTH, MAX_HEALTH, MIN_X_SIZE, MAX_X_SIZE);

        this.texture_progress_bar.CustomMinimumSize = new Vector2(newXSize, this.texture_progress_bar.CustomMinimumSize.Y);
        this.texture_progress_bar.MaxValue = value;
        this.CustomMinimumSize = new Vector2(newXSize, this.CustomMinimumSize.Y);
    }

    public void update_health(float new_value)
    {
        this.texture_progress_bar.Value = new_value;
    }

    public void set_debuffs(Array<EnemyDebuffInstance> debuffs)
    {
        this._reset_debuffs();

        for (int i = 0; i < debuffs.Count; i++)
        {
            EnemyDebuffInstance debuffInstance = debuffs[i];
            int debuffType = (int)debuffInstance.debuff.type;
            int current = this.debuffs_count.ContainsKey(debuffType) ? this.debuffs_count[debuffType] : 0;
            this.debuffs_count[debuffType] = current + 1;
            this.debuff_data_by_type[debuffType] = debuffInstance.debuff.data;
        }

        List<int> slotKeys = new(this.debuffs_slots.Keys);
        for (int i = 0; i < slotKeys.Count; i++)
        {
            int type = slotKeys[i];
            if (!this.debuffs_count.ContainsKey(type))
            {
                this._remove_debuff(type);
            }
        }

        List<int> keys = new(this.debuffs_count.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            int type = keys[i];
            this._update_debuff_value(type, this.debuffs_count[type], this.debuff_data_by_type[type]);
        }
    }

    private void _reset_debuffs()
    {
        this.debuffs_count = new System.Collections.Generic.Dictionary<int, int>();
        this.debuff_data_by_type = new System.Collections.Generic.Dictionary<int, Variant>();
    }

    private void _update_debuff_value(int type, int value, Variant debuff_data)
    {
        DebuffSlot slot = this.debuffs_slots.ContainsKey(type) ? this.debuffs_slots[type] : null;
        if (slot == null)
        {
            this._create_debuff_slot_type(type, debuff_data.AsGodotObject());
        }

        this.debuffs_slots[type].amount = value;
    }

    private void _create_debuff_slot_type(int type, GodotObject debuff_data)
    {
        DebuffSlot slot = DEBUFF_SLOT.Instantiate<DebuffSlot>();
        this.debuffs_conatiner.AddChild(slot);
        slot.texture = debuff_data?.Get("icon").As<Texture2D>();
        this.debuffs_slots[type] = slot;
    }

    private void _remove_debuff(int type)
    {
        DebuffSlot slot = this.debuffs_slots.ContainsKey(type) ? this.debuffs_slots[type] : null;
        if (slot == null)
        {
            return;
        }

        slot.QueueFree();
        this.debuffs_slots.Remove(type);
    }
}
