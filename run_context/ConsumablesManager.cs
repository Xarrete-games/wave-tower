using Godot;
using System.Collections.Generic;
using System;

public class ConsumablesManager
{
    public event Action<List<Consumable>> consumables_change;
    public event Action<Consumable> consumable_added;
    public event Action<Consumable> consumable_used;
    public event Action<Consumable> consumable_clicked;

    private readonly List<Consumable> _consumables = new();
    private readonly Dictionary<Consumable, ConsumableModel> _runtimeConsumableModels = new();

    public bool is_full()
    {
        return _consumables.Count == 5;
    }

    public void add_consumable(Consumable consumable)
    {
        if (is_full())
        {
            return;
        }

        if (consumable == null)
        {
            return;
        }

        _consumables.Add(consumable);
        SyncConsumableAddedToRuntime(consumable);
        consumable.clicked += _on_consumable_clicked;
        consumable.used += _on_consumable_used;
        consumables_change?.Invoke(_consumables);
        consumable_added?.Invoke(consumable);
    }

    public void _on_consumable_used(Consumable consumable)
    {
        ConsumableModel consumableModel = null;
        _runtimeConsumableModels.TryGetValue(consumable, out consumableModel);

        if (consumableModel != null)
        {
            SyncConsumableUseTargetToRuntime(consumable, consumableModel);
            SyncRuntimeStatusFromLegacy();
            Hooks.OnConsumableUsed(Hooks.GetListenersFromRuntime(), consumableModel);
            SyncLegacyStatusFromRuntime();
            SyncTowerBuffsFromConsumableTarget(consumable);
        }

        if (consumable != null)
        {
            consumable.clicked -= _on_consumable_clicked;
            consumable.used -= _on_consumable_used;
            SyncConsumableRemovedFromRuntime(consumable);
        }

        consumable_used?.Invoke(consumable);
        _consumables.Remove(consumable);

        bool recoveredByHooks = consumableModel != null && IsConsumablePresentInRuntime(consumableModel);
        if (recoveredByHooks && consumable != null && !is_full())
        {
            _runtimeConsumableModels[consumable] = consumableModel;
            _consumables.Add(consumable);
            consumable.clicked += _on_consumable_clicked;
            consumable.used += _on_consumable_used;
            consumable_added?.Invoke(consumable);
        }

        consumables_change?.Invoke(_consumables);
    }

    public void _on_consumable_clicked(Consumable consumable)
    {
        if (consumable == null)
        {
            return;
        }

        bool requiresTarget = consumable.requires_target();
        if (!requiresTarget)
        {
            if (consumable is ConsumableUsable usable)
            {
                usable.use();
            }

            consumable.emit_used();
        }
        else
        {
            consumable_clicked?.Invoke(consumable);
        }
    }

    private void SyncConsumableAddedToRuntime(Consumable consumableObj)
    {
        if (consumableObj == null)
        {
            return;
        }

        ConsumableModel model = BuildConsumableModel(consumableObj);
        if (model == null)
        {
            return;
        }

        _runtimeConsumableModels[consumableObj] = model;
        RunContextRuntime.ConsumablesManager.AddConsumable(model);
    }

    private void SyncConsumableRemovedFromRuntime(Consumable consumableObj)
    {
        if (consumableObj == null)
        {
            return;
        }

        if (!_runtimeConsumableModels.TryGetValue(consumableObj, out ConsumableModel model))
        {
            return;
        }

        RunContextRuntime.ConsumablesManager.RemoveConsumable(model);
        _runtimeConsumableModels.Remove(consumableObj);
    }

    private bool IsConsumablePresentInRuntime(ConsumableModel model)
    {
        if (model == null)
        {
            return false;
        }

        var runtimeConsumables = RunContextRuntime.ConsumablesManager.GetConsumables();
        for (int index = 0; index < runtimeConsumables.Count; index++)
        {
            if (ReferenceEquals(runtimeConsumables[index], model))
            {
                return true;
            }
        }

        return false;
    }

    private void SyncRuntimeStatusFromLegacy()
    {
        Status status = GetRunContext()?.status;
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.MaxHealth, status.health, status.armor);
    }

    private void SyncLegacyStatusFromRuntime()
    {
        Status status = GetRunContext()?.status;
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.MaxHealth != runtime.MaxHealth)
        {
            status.MaxHealth = runtime.MaxHealth;
        }

        if (status.armor != runtime.Armor)
        {
            status.armor = runtime.Armor;
        }

        if (status.health != runtime.Health)
        {
            status.health = runtime.Health;
        }
    }

    private void SyncTowerBuffsFromConsumableTarget(Consumable consumableObj)
    {
        if (consumableObj == null)
        {
            return;
        }

        Tower targetTower = (consumableObj as ConsumableTargeteable)?.get_target_tower();

        if (targetTower == null)
        {
            return;
        }

        TowersManager towersManager = GetRunContext()?.towers_manager;
        if (towersManager == null)
        {
            return;
        }

        towersManager.sync_runtime_buffs_for_tower(targetTower.GetInstanceId());
    }

    private void SyncConsumableUseTargetToRuntime(Consumable consumableObj, ConsumableModel consumableModel)
    {
        if (consumableObj == null || consumableModel is not ConsumableTargeteableModel targetModel)
        {
            return;
        }

        if (targetModel.TargetingType != ConsumableTargeteableModel.TargetType.Tower)
        {
            return;
        }

        if (TryGetTargetTowerModelFromConsumable(consumableObj, out TowerModel towerModel))
        {
            targetModel.TargetTower = towerModel;
        }
    }

    private bool TryGetTargetTowerModelFromConsumable(Consumable consumableObj, out TowerModel towerModel)
    {
        towerModel = null;
        if (consumableObj is not ConsumableTargeteable targeteable)
        {
            return false;
        }

        Tower targetTower = targeteable.get_target_tower();
        if (targetTower == null)
        {
            return false;
        }

        return RunContextRuntime.TowersManager.TryGetTowerByInstanceId(targetTower.GetInstanceId(), out towerModel);
    }

    private RunContext GetRunContext()
    {
        return (Engine.GetMainLoop() as SceneTree)?.Root.GetNodeOrNull<RunContext>("/root/RunContext");
    }

    private ConsumableModel BuildConsumableModel(Consumable consumableObj)
    {
        ConsumableData data = consumableObj.data;
        if (data == null)
        {
            return null;
        }

        string id = data.Id;
        int consumableTypeRaw = data.ConsumableType;
        ConsumableModel.ConsumableType consumableType = consumableTypeRaw == 1
            ? ConsumableModel.ConsumableType.Potion
            : ConsumableModel.ConsumableType.Other;

        bool requiresTarget = consumableObj.requires_target();
        if (!requiresTarget)
        {
            return new SimpleConsumableModel(id, consumableType);
        }

        int targetTypeRaw = data.TargetingType;
        ConsumableTargeteableModel.TargetType targetType = targetTypeRaw == 1
            ? ConsumableTargeteableModel.TargetType.Tower
            : ConsumableTargeteableModel.TargetType.BlockedTile;

        var model = new ConsumableTargeteableModel(id, targetType);
        if (targetType == ConsumableTargeteableModel.TargetType.Tower)
        {
            if (TryGetTargetTowerModelFromConsumable(consumableObj, out TowerModel towerModel))
            {
                model.TargetTower = towerModel;
            }
        }

        return model;
    }
}

