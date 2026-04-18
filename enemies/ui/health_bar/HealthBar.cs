using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class HealthBar : Control
{
    private static readonly PackedScene DEBUFF_SLOT = GD.Load<PackedScene>("uid://beixhmpysku3t");

    private const float MIN_HEALTH = 40.0f;
    private const float MAX_HEALTH = 5000.0f;
    private const float MIN_X_SIZE = 40.0f;
    private const float MAX_X_SIZE = 160.0f;

    [Export] public Control DebuffsConatiner;
    [Export] public TextureProgressBar TextureProgressBar;

    public System.Collections.Generic.Dictionary<int, DebuffSlot> debuffs_slots = new();
    public System.Collections.Generic.Dictionary<int, int> debuffs_count = new();
    public System.Collections.Generic.Dictionary<int, EnemyDebuffData> debuff_data_by_type = new();

    public void set_MaxHealth(float value)
    {
        float clampedValue = Mathf.Clamp(value, MIN_HEALTH, MAX_HEALTH);
        float newXSize = Mathf.Remap(clampedValue, MIN_HEALTH, MAX_HEALTH, MIN_X_SIZE, MAX_X_SIZE);

        TextureProgressBar.CustomMinimumSize = new Vector2(newXSize, TextureProgressBar.CustomMinimumSize.Y);
        TextureProgressBar.MaxValue = value;
        CustomMinimumSize = new Vector2(newXSize, CustomMinimumSize.Y);
    }

    public void update_health(float new_value)
    {
        TextureProgressBar.Value = new_value;
    }

    public void set_debuffs(List<EnemyDebuffInstance> debuffs)
    {
        ResetDebuffs();

        for (int i = 0; i < debuffs.Count; i++)
        {
            EnemyDebuffInstance debuffInstance = debuffs[i];
            int debuffType = (int)debuffInstance.debuff.type;
            int current = debuffs_count.ContainsKey(debuffType) ? debuffs_count[debuffType] : 0;
            debuffs_count[debuffType] = current + 1;
            debuff_data_by_type[debuffType] = debuffInstance.debuff.data;
        }

        List<int> slotKeys = new(debuffs_slots.Keys);
        for (int i = 0; i < slotKeys.Count; i++)
        {
            int type = slotKeys[i];
            if (!debuffs_count.ContainsKey(type))
            {
                RemoveDebuff(type);
            }
        }

        List<int> keys = new(debuffs_count.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            int type = keys[i];
            UpdateDebuffValue(type, debuffs_count[type], debuff_data_by_type[type]);
        }
    }

    private void ResetDebuffs()
    {
        debuffs_count = new System.Collections.Generic.Dictionary<int, int>();
        debuff_data_by_type = new System.Collections.Generic.Dictionary<int, EnemyDebuffData>();
    }

    private void UpdateDebuffValue(int type, int value, EnemyDebuffData debuff_data)
    {
        DebuffSlot slot = debuffs_slots.ContainsKey(type) ? debuffs_slots[type] : null;
        if (slot == null)
        {
            CreateDebuffSlotType(type, debuff_data);
        }

        debuffs_slots[type].amount = value;
    }

    private void CreateDebuffSlotType(int type, EnemyDebuffData debuff_data)
    {
        DebuffSlot slot = DEBUFF_SLOT.Instantiate<DebuffSlot>();
        DebuffsConatiner.AddChild(slot);
        slot.texture = debuff_data?.Icon;
        debuffs_slots[type] = slot;
    }

    private void RemoveDebuff(int type)
    {
        DebuffSlot slot = debuffs_slots.ContainsKey(type) ? debuffs_slots[type] : null;
        if (slot == null)
        {
            return;
        }

        slot.QueueFree();
        debuffs_slots.Remove(type);
    }
}

