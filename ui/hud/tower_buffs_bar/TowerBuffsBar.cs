using Godot;
using System.Collections.Generic;

public partial class TowerBuffsBar : Control
{
    [Export]
    public PackedScene SlotScene;

    [Export]
    public NodePath SlotsContainer;

    private Tower _tower;
    private readonly List<TowerBuff> _towerBuffs = new();
    private readonly Dictionary<string, int> _buffsModifiersStacks = new();

    private Control _slotsContainerNode;

    public override void _Ready()
    {
        _tower = GetParent() as Tower;
        _slotsContainerNode = !SlotsContainer.IsEmpty ? GetNodeOrNull<Control>(SlotsContainer) : GetNodeOrNull<Control>("Container");

        if (_tower == null)
        {
            return;
        }

        _tower.BuffAdded += OnTowerBuffAdded;
        _tower.BuffRemoved += OnTowerBuffRemoved;
    }

    public override void _ExitTree()
    {
        if (_tower != null)
        {
            _tower.BuffAdded -= OnTowerBuffAdded;
            _tower.BuffRemoved -= OnTowerBuffRemoved;
        }

        _towerBuffs.Clear();
        _buffsModifiersStacks.Clear();
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
        _towerBuffs.Add(buff);

        if (!IsStatsModifier(buff))
        {
            return;
        }

        int modifierValue = (buff as TowerBuffStatsModifier)?.value ?? 0;
        if (!buffExists)
        {
            _buffsModifiersStacks[buffId] = modifierValue;
            TowerBuffsBarSlot slot = SlotScene?.Instantiate() as TowerBuffsBarSlot;
            if (slot == null || _slotsContainerNode == null)
            {
                return;
            }

            _slotsContainerNode.AddChild(slot);
            slot.SetBuff(buff, modifierValue);
            return;
        }

        _buffsModifiersStacks[buffId] = _buffsModifiersStacks.GetValueOrDefault(buffId, 0) + modifierValue;
        foreach (Node slotNode in _slotsContainerNode.GetChildren())
        {
            TowerBuffsBarSlot buffSlot = slotNode as TowerBuffsBarSlot;
            if (buffSlot == null || buffSlot.TowerBuff == null)
            {
                continue;
            }

            string slotBuffId = GetBuffId(buffSlot.TowerBuff);
            if (slotBuffId == buffId)
            {
                buffSlot.Value = _buffsModifiersStacks[buffId];
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
        foreach (TowerBuff remainingBuff in _towerBuffs)
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
            if (buffSlot == null || buffSlot.TowerBuff == null)
            {
                continue;
            }

            string slotBuffId = GetBuffId(buffSlot.TowerBuff);
            if (slotBuffId != buffId)
            {
                continue;
            }

            if (hasSameBuffInstance)
            {
                _buffsModifiersStacks[buffId] = totalValue;
                buffSlot.Value = totalValue;
            }
            else
            {
                buffSlot.QueueFree();
                _buffsModifiersStacks.Remove(buffId);
            }

            break;
        }
    }

    private void EraseFirstBuffInstance(TowerBuff removedBuff)
    {
        for (int i = 0; i < _towerBuffs.Count; i++)
        {
            TowerBuff candidate = _towerBuffs[i];
            if (ReferenceEquals(candidate, removedBuff))
            {
                _towerBuffs.RemoveAt(i);
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
        foreach (TowerBuff buff in _towerBuffs)
        {
            if (buff != null && GetBuffId(buff) == buffId)
            {
                return true;
            }
        }

        return false;
    }
}
