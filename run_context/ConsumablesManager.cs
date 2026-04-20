using Godot;
using System.Collections.Generic;
using System;

public class ConsumablesManager
{
    public event Action<List<Consumable>> ConsumablesChanged;
    public event Action<Consumable> ConsumableAdded;
    public event Action<Consumable> ConsumableUsed;
    public event Action<Consumable> ConsumableClicked;

    private readonly List<Consumable> _consumables = new();
    private readonly Dictionary<Consumable, ConsumableModel> _runtimeConsumableModels = new();

    public bool IsFull()
    {
        return _consumables.Count == 5;
    }

    public void AddConsumable(Consumable consumable)
    {
        if (IsFull())
        {
            return;
        }

        if (consumable == null)
        {
            return;
        }

        _consumables.Add(consumable);
        SyncConsumableAddedToRuntime(consumable);
        consumable.clicked += OnConsumableClicked;
        consumable.used += OnConsumableUsed;
        ConsumablesChanged?.Invoke(_consumables);
        ConsumableAdded?.Invoke(consumable);
    }

    public void OnConsumableUsed(Consumable consumable)
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
            consumable.clicked -= OnConsumableClicked;
            consumable.used -= OnConsumableUsed;
            SyncConsumableRemovedFromRuntime(consumable);
        }

        ConsumableUsed?.Invoke(consumable);
        _consumables.Remove(consumable);

        bool recoveredByHooks = consumableModel != null && IsConsumablePresentInRuntime(consumableModel);
        if (recoveredByHooks && consumable != null && !IsFull())
        {
            _runtimeConsumableModels[consumable] = consumableModel;
            _consumables.Add(consumable);
            consumable.clicked += OnConsumableClicked;
            consumable.used += OnConsumableUsed;
            ConsumableAdded?.Invoke(consumable);
        }

        ConsumablesChanged?.Invoke(_consumables);
    }

    public void OnConsumableClicked(Consumable consumable)
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
            ConsumableClicked?.Invoke(consumable);
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
        Status status = GetRunContext()?.Status;
        if (status == null)
        {
            return;
        }

        RunContextRuntime.Status.SyncFromLegacy(status.MaxHealth, status.Health, status.Armor);
    }

    private void SyncLegacyStatusFromRuntime()
    {
        Status status = GetRunContext()?.Status;
        if (status == null)
        {
            return;
        }

        StatusRuntime runtime = RunContextRuntime.Status;
        if (status.MaxHealth != runtime.MaxHealth)
        {
            status.MaxHealth = runtime.MaxHealth;
        }

        if (status.Armor != runtime.Armor)
        {
            status.Armor = runtime.Armor;
        }

        if (status.Health != runtime.Health)
        {
            status.Health = runtime.Health;
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

        TowersManager towersManager = GetRunContext()?.TowersManager;
        if (towersManager == null)
        {
            return;
        }

        towersManager.SyncRuntimeBuffsForTower(targetTower.GetInstanceId());
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

