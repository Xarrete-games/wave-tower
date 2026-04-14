using Godot;
using System.Collections.Generic;

public partial class TowerBuffsBar : Control
{
    [Export]
    public PackedScene slot_scene;

    [Export]
    public NodePath slots_container;

    private Tower tower;
    private readonly List<TowerBuff> tower_buffs = new();
    private readonly Dictionary<string, int> buffs_modifiers_stacks = new();

    private Control _slotsContainerNode;

    public override void _Ready()
    {
        this.tower = GetParent() as Tower;
        this._slotsContainerNode = !this.slots_container.IsEmpty ? GetNodeOrNull<Control>(this.slots_container) : GetNodeOrNull<Control>("Container");

        if (this.tower == null)
        {
            return;
        }

        this.tower.buff_added += this._on_tower_buff_added;
        this.tower.buff_removed += this._on_tower_buff_removed;
    }

    public override void _ExitTree()
    {
        if (this.tower != null)
        {
            this.tower.buff_added -= this._on_tower_buff_added;
            this.tower.buff_removed -= this._on_tower_buff_removed;
        }

        this.tower_buffs.Clear();
        this.buffs_modifiers_stacks.Clear();
    }

    private void _on_tower_buff_added(TowerBuff buff)
    {
        if (buff == null)
        {
            return;
        }

        string buffId = this._get_buff_id(buff);
        if (string.IsNullOrEmpty(buffId))
        {
            return;
        }

        bool buffExists = this._buff_exists(buffId);
        this.tower_buffs.Add(buff);

        if (!this._is_stats_modifier(buff))
        {
            return;
        }

        int modifierValue = (buff as TowerBuffStatsModifier)?.value ?? 0;
        if (!buffExists)
        {
            this.buffs_modifiers_stacks[buffId] = modifierValue;
            TowerBuffsBarSlot slot = this.slot_scene?.Instantiate() as TowerBuffsBarSlot;
            if (slot == null || this._slotsContainerNode == null)
            {
                return;
            }

            this._slotsContainerNode.AddChild(slot);
            slot.set_buff(buff, modifierValue);
            return;
        }

        this.buffs_modifiers_stacks[buffId] = this.buffs_modifiers_stacks.GetValueOrDefault(buffId, 0) + modifierValue;
        foreach (Node slotNode in this._slotsContainerNode.GetChildren())
        {
            TowerBuffsBarSlot buffSlot = slotNode as TowerBuffsBarSlot;
            if (buffSlot == null || buffSlot.tower_buff == null)
            {
                continue;
            }

            string slotBuffId = this._get_buff_id(buffSlot.tower_buff);
            if (slotBuffId == buffId)
            {
                buffSlot.value = this.buffs_modifiers_stacks[buffId];
            }
        }
    }

    private void _on_tower_buff_removed(TowerBuff removedBuff)
    {
        if (removedBuff == null)
        {
            return;
        }

        string buffId = this._get_buff_id(removedBuff);
        if (string.IsNullOrEmpty(buffId))
        {
            return;
        }

        this._erase_first_buff_instance(removedBuff);

        bool hasSameBuffInstance = false;
        int totalValue = 0;
        foreach (TowerBuff remainingBuff in this.tower_buffs)
        {
            if (remainingBuff == null || this._get_buff_id(remainingBuff) != buffId)
            {
                continue;
            }

            hasSameBuffInstance = true;
            if (this._is_stats_modifier(remainingBuff))
            {
                totalValue += (remainingBuff as TowerBuffStatsModifier)?.value ?? 0;
            }
        }

        foreach (Node slotNode in this._slotsContainerNode.GetChildren())
        {
            TowerBuffsBarSlot buffSlot = slotNode as TowerBuffsBarSlot;
            if (buffSlot == null || buffSlot.tower_buff == null)
            {
                continue;
            }

            string slotBuffId = this._get_buff_id(buffSlot.tower_buff);
            if (slotBuffId != buffId)
            {
                continue;
            }

            if (hasSameBuffInstance)
            {
                this.buffs_modifiers_stacks[buffId] = totalValue;
                buffSlot.value = totalValue;
            }
            else
            {
                buffSlot.QueueFree();
                this.buffs_modifiers_stacks.Remove(buffId);
            }

            break;
        }
    }

    private void _erase_first_buff_instance(TowerBuff removedBuff)
    {
        for (int i = 0; i < this.tower_buffs.Count; i++)
        {
            TowerBuff candidate = this.tower_buffs[i];
            if (ReferenceEquals(candidate, removedBuff))
            {
                this.tower_buffs.RemoveAt(i);
                return;
            }
        }
    }

    private bool _is_stats_modifier(TowerBuff buff)
    {
        return buff is TowerBuffStatsModifier;
    }

    private string _get_buff_id(TowerBuff buff)
    {
        return buff?.data?.id ?? string.Empty;
    }

    private bool _buff_exists(string buffId)
    {
        foreach (TowerBuff buff in this.tower_buffs)
        {
            if (buff != null && this._get_buff_id(buff) == buffId)
            {
                return true;
            }
        }

        return false;
    }
}
