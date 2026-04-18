using Godot;
using System.Collections.Generic;

public partial class TowerBuffsBar : Control
{
    [Export]
    public PackedScene SlotScene;

    [Export]
    public NodePath SlotsContainer;

    private Tower tower;
    private readonly List<TowerBuff> tower_buffs = new();
    private readonly Dictionary<string, int> buffs_modifiers_stacks = new();

    private Control _slotsContainerNode;

    public override void _Ready()
    {
        tower = GetParent() as Tower;
        _slotsContainerNode = !SlotsContainer.IsEmpty ? GetNodeOrNull<Control>(SlotsContainer) : GetNodeOrNull<Control>("Container");

        if (tower == null)
        {
            return;
        }

        tower.BuffAdded += OnTowerBuffAdded;
        tower.BuffRemoved += OnTowerBuffRemoved;
    }

    public override void _ExitTree()
    {
        if (tower != null)
        {
            tower.BuffAdded -= OnTowerBuffAdded;
            tower.BuffRemoved -= OnTowerBuffRemoved;
        }

        tower_buffs.Clear();
        buffs_modifiers_stacks.Clear();
    }

    private void OnTowerBuffAdded(TowerBuff buff)
    {
        if (buff == null)
        {
            return;
        }

        string buffId = GetBuffId(buff);
        if (string.IsNullOrEmpty(buffId))
        {
            return;
        }

        bool buffExists = BuffExists(buffId);
        tower_buffs.Add(buff);

        if (!IsStatsModifier(buff))
        {
            return;
        }

        int modifierValue = (buff as TowerBuffStatsModifier)?.value ?? 0;
        if (!buffExists)
        {
            buffs_modifiers_stacks[buffId] = modifierValue;
            TowerBuffsBarSlot slot = SlotScene?.Instantiate() as TowerBuffsBarSlot;
            if (slot == null || _slotsContainerNode == null)
            {
                return;
            }

            _slotsContainerNode.AddChild(slot);
            slot.set_buff(buff, modifierValue);
            return;
        }

        buffs_modifiers_stacks[buffId] = buffs_modifiers_stacks.GetValueOrDefault(buffId, 0) + modifierValue;
        foreach (Node slotNode in _slotsContainerNode.GetChildren())
        {
            TowerBuffsBarSlot buffSlot = slotNode as TowerBuffsBarSlot;
            if (buffSlot == null || buffSlot.tower_buff == null)
            {
                continue;
            }

            string slotBuffId = GetBuffId(buffSlot.tower_buff);
            if (slotBuffId == buffId)
            {
                buffSlot.value = buffs_modifiers_stacks[buffId];
            }
        }
    }

    private void OnTowerBuffRemoved(TowerBuff removedBuff)
    {
        if (removedBuff == null)
        {
            return;
        }

        string buffId = GetBuffId(removedBuff);
        if (string.IsNullOrEmpty(buffId))
        {
            return;
        }

        EraseFirstBuffInstance(removedBuff);

        bool hasSameBuffInstance = false;
        int totalValue = 0;
        foreach (TowerBuff remainingBuff in tower_buffs)
        {
            if (remainingBuff == null || GetBuffId(remainingBuff) != buffId)
            {
                continue;
            }

            hasSameBuffInstance = true;
            if (IsStatsModifier(remainingBuff))
            {
                totalValue += (remainingBuff as TowerBuffStatsModifier)?.value ?? 0;
            }
        }

        foreach (Node slotNode in _slotsContainerNode.GetChildren())
        {
            TowerBuffsBarSlot buffSlot = slotNode as TowerBuffsBarSlot;
            if (buffSlot == null || buffSlot.tower_buff == null)
            {
                continue;
            }

            string slotBuffId = GetBuffId(buffSlot.tower_buff);
            if (slotBuffId != buffId)
            {
                continue;
            }

            if (hasSameBuffInstance)
            {
                buffs_modifiers_stacks[buffId] = totalValue;
                buffSlot.value = totalValue;
            }
            else
            {
                buffSlot.QueueFree();
                buffs_modifiers_stacks.Remove(buffId);
            }

            break;
        }
    }

    private void EraseFirstBuffInstance(TowerBuff removedBuff)
    {
        for (int i = 0; i < tower_buffs.Count; i++)
        {
            TowerBuff candidate = tower_buffs[i];
            if (ReferenceEquals(candidate, removedBuff))
            {
                tower_buffs.RemoveAt(i);
                return;
            }
        }
    }

    private bool IsStatsModifier(TowerBuff buff)
    {
        return buff is TowerBuffStatsModifier;
    }

    private string GetBuffId(TowerBuff buff)
    {
        return buff?.data?.Id ?? string.Empty;
    }

    private bool BuffExists(string buffId)
    {
        foreach (TowerBuff buff in tower_buffs)
        {
            if (buff != null && GetBuffId(buff) == buffId)
            {
                return true;
            }
        }

        return false;
    }
}
