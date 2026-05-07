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

    public System.Collections.Generic.Dictionary<int, DebuffSlot> DebuffsSlots = new();
    public System.Collections.Generic.Dictionary<int, int> DebuffsCount = new();
    public System.Collections.Generic.Dictionary<int, EnemyDebuffData> DebuffDataByType = new();

    public void SetMaxHealth(float value)
    {
        float clampedValue = Mathf.Clamp(value, MIN_HEALTH, MAX_HEALTH);
        float newXSize = Mathf.Remap(clampedValue, MIN_HEALTH, MAX_HEALTH, MIN_X_SIZE, MAX_X_SIZE);

        TextureProgressBar.CustomMinimumSize = new Vector2(newXSize, TextureProgressBar.CustomMinimumSize.Y);
        TextureProgressBar.MaxValue = value;
        CustomMinimumSize = new Vector2(newXSize, CustomMinimumSize.Y);
    }

    public void UpdateHealth(float newValue)
    {
        TextureProgressBar.Value = newValue;
    }

    public void SetDebuffs(List<EnemyDebuffInstance> debuffs)
    {
        ResetDebuffs();

        for (int i = 0; i < debuffs.Count; i++)
        {
            EnemyDebuffInstance debuffInstance = debuffs[i];
            int debuffType = (int)debuffInstance.Debuff.DebuffType;
            int current = DebuffsCount.ContainsKey(debuffType) ? DebuffsCount[debuffType] : 0;
            DebuffsCount[debuffType] = current + 1;
            DebuffDataByType[debuffType] = debuffInstance.Debuff.Data;
        }

        List<int> slotKeys = new(DebuffsSlots.Keys);
        for (int i = 0; i < slotKeys.Count; i++)
        {
            int type = slotKeys[i];
            if (!DebuffsCount.ContainsKey(type))
            {
                RemoveDebuff(type);
            }
        }

        List<int> keys = new(DebuffsCount.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            int type = keys[i];
            UpdateDebuffValue(type, DebuffsCount[type], DebuffDataByType[type]);
        }
    }

    private void ResetDebuffs()
    {
        DebuffsCount = new System.Collections.Generic.Dictionary<int, int>();
        DebuffDataByType = new System.Collections.Generic.Dictionary<int, EnemyDebuffData>();
    }

    private void UpdateDebuffValue(int type, int value, EnemyDebuffData debuffData)
    {
        DebuffSlot slot = DebuffsSlots.ContainsKey(type) ? DebuffsSlots[type] : null;
        if (slot == null)
        {
            CreateDebuffSlotType(type, debuffData);
        }

        DebuffsSlots[type].amount = value;
    }

    private void CreateDebuffSlotType(int type, EnemyDebuffData debuffData)
    {
        DebuffSlot slot = DEBUFF_SLOT.Instantiate<DebuffSlot>();
        DebuffsConatiner.AddChild(slot);
        slot.texture = debuffData?.Icon;
        DebuffsSlots[type] = slot;
    }

    private void RemoveDebuff(int type)
    {
        DebuffSlot slot = DebuffsSlots.ContainsKey(type) ? DebuffsSlots[type] : null;
        if (slot == null)
        {
            return;
        }

        slot.QueueFree();
        DebuffsSlots.Remove(type);
    }
}

