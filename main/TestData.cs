using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public partial class TestData : Node
{
    [Export]
    public int InitialRandomRelics = 0;

    [Export]
    public string[] InitialRelicsIds = Array.Empty<string>();

    [Export]
    public string[] InitialConsumablesIds = Array.Empty<string>();

    [Export]
    public EventData InitialEvent;

    [Export]
    public bool OpenLootScreen = false;

    private RunHandler _runHandler;

    public override void _Ready()
    {
        AsyncTaskHelper.FireAndForget(ReadyAsync(), "TestData.ReadyAsync");
    }

    private async Task ReadyAsync()
    {
        _runHandler = GetNodeOrNull<RunHandler>("../RunHandler");

        await ToSignal(GetTree().CreateTimer(0.1f, false), SceneTreeTimer.SignalName.Timeout);
        HandleInitialRelics();

        if (OpenLootScreen && _runHandler != null)
        {
            await _runHandler.ShowLootScreen();
        }

        if (InitialEvent != null && _runHandler != null)
        {
            await _runHandler.ShowEventsScreen(InitialEvent);
        }
    }

    private void HandleInitialRelics()
    {
        RunContext runContext = RunContext.Instance;

        if (InitialRelicsIds.Length > 0)
        {
            for (int index = 0; index < InitialRelicsIds.Length; index++)
            {
                string relicId = InitialRelicsIds[index];
                RelicData typedRelicData = DataLoaderAccess.GetRelicById(relicId);
                if (typedRelicData != null)
                {
                    Relic relicInstance = typedRelicData.CreateItem();
                    runContext.RelicsManager.AddRelic(relicInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial relic id {relicId} not found");
                }
            }
        }

        if (InitialRandomRelics > 0)
        {
            System.Collections.Generic.List<RelicData> items = DataLoaderAccess.GetRandomRelicsTyped(InitialRandomRelics);
            for (int index = 0; index < items.Count; index++)
            {
                RelicData relicDataObj = items[index];
                if (relicDataObj == null)
                {
                    continue;
                }

                Relic relic = relicDataObj.CreateItem();
                runContext.RelicsManager.AddRelic(relic);
            }
        }

        if (InitialConsumablesIds.Length > 0)
        {
            for (int index = 0; index < InitialConsumablesIds.Length; index++)
            {
                string consumableId = InitialConsumablesIds[index];
                ConsumableData consumableDataObj = DataLoaderAccess.GetConsumableById(consumableId);
                if (consumableDataObj != null)
                {
                    Consumable consumableInstance = consumableDataObj.CreateConsumable();
                    runContext.ConsumablesManager.AddConsumable(consumableInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial consumable id {consumableId} not found");
                }
            }
        }
    }
}
