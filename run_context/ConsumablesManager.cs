using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ConsumablesManager : RefCounted
{
    [Signal]
    public delegate void consumables_changeEventHandler(Godot.Collections.Array<Variant> consumables);

    [Signal]
    public delegate void consumable_addedEventHandler(Variant consumable);

    [Signal]
    public delegate void consumable_usedEventHandler(Variant consumable);

    [Signal]
    public delegate void consumable_clickedEventHandler(Variant consumable);

    private static readonly Script _hooksScript = GD.Load<Script>("res://core/hooks.gd");
    private readonly Godot.Collections.Array<Variant> _consumables = new();
    private readonly Dictionary<ulong, ConsumableModel> _runtimeConsumableModels = new();

    public bool is_full()
    {
        return this._consumables.Count == 5;
    }

    public void add_consumable(Variant consumable)
    {
        if (this.is_full())
        {
            return;
        }

        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj == null)
        {
            return;
        }

        this._consumables.Add(consumable);
        this.SyncConsumableAddedToRuntime(consumableObj);
        consumableObj.Connect("clicked", Callable.From<Variant>(this._on_consumable_clicked));
        consumableObj.Connect("used", Callable.From<Variant>(this._on_consumable_used));
        this.EmitSignal(SignalName.consumables_change, this._consumables);
        this.EmitSignal(SignalName.consumable_added, consumable);
    }

    public void _on_consumable_used(Variant consumable)
    {
        _hooksScript.Call("on_consumable_used", consumable);

        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj != null)
        {
            consumableObj.Disconnect("clicked", Callable.From<Variant>(this._on_consumable_clicked));
            consumableObj.Disconnect("used", Callable.From<Variant>(this._on_consumable_used));
            this.SyncConsumableRemovedFromRuntime(consumableObj);
        }

        this.EmitSignal(SignalName.consumable_used, consumable);
        this._consumables.Remove(consumable);
        this.EmitSignal(SignalName.consumables_change, this._consumables);
    }

    public void _on_consumable_clicked(Variant consumable)
    {
        GodotObject consumableObj = consumable.AsGodotObject();
        if (consumableObj == null)
        {
            return;
        }

        bool requiresTarget = consumableObj.HasMethod("requires_target") && (bool)consumableObj.Call("requires_target");
        if (!requiresTarget)
        {
            consumableObj.Call("use");
            consumableObj.EmitSignal("used", consumable);
        }
        else
        {
            this.EmitSignal(SignalName.consumable_clicked, consumable);
        }
    }

    private void SyncConsumableAddedToRuntime(GodotObject consumableObj)
    {
        if (consumableObj == null)
        {
            return;
        }

        ulong instanceId = consumableObj.GetInstanceId();
        ConsumableModel model = this.BuildConsumableModel(consumableObj);
        if (model == null)
        {
            return;
        }

        this._runtimeConsumableModels[instanceId] = model;
        RunContextRuntime.ConsumablesManager.AddConsumable(model);
    }

    private void SyncConsumableRemovedFromRuntime(GodotObject consumableObj)
    {
        if (consumableObj == null)
        {
            return;
        }

        ulong instanceId = consumableObj.GetInstanceId();
        if (!this._runtimeConsumableModels.TryGetValue(instanceId, out ConsumableModel model))
        {
            return;
        }

        RunContextRuntime.ConsumablesManager.RemoveConsumable(model);
        this._runtimeConsumableModels.Remove(instanceId);
    }

    private ConsumableModel BuildConsumableModel(GodotObject consumableObj)
    {
        GodotObject data = consumableObj.Get("data").AsGodotObject();
        if (data == null)
        {
            return null;
        }

        string id = data.Get("id").AsString();
        int consumableTypeRaw = (int)data.Get("consumable_type");
        ConsumableModel.ConsumableType consumableType = consumableTypeRaw == 1
            ? ConsumableModel.ConsumableType.Potion
            : ConsumableModel.ConsumableType.Other;

        bool requiresTarget = consumableObj.HasMethod("requires_target") && (bool)consumableObj.Call("requires_target");
        if (!requiresTarget)
        {
            return new SimpleConsumableModel(id, consumableType);
        }

        int targetTypeRaw = (int)data.Get("targeting_type");
        ConsumableTargeteableModel.TargetType targetType = targetTypeRaw == 1
            ? ConsumableTargeteableModel.TargetType.Tower
            : ConsumableTargeteableModel.TargetType.BlockedTile;

        var model = new ConsumableTargeteableModel(id, targetType);
        if (targetType == ConsumableTargeteableModel.TargetType.Tower)
        {
            GodotObject targetTower = consumableObj.Get("target").AsGodotObject();
            if (targetTower != null && RunContextRuntime.TowersManager.TryGetTowerByInstanceId(targetTower.GetInstanceId(), out TowerModel towerModel))
            {
                model.TargetTower = towerModel;
            }
        }

        return model;
    }
}
