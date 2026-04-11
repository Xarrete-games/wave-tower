using Godot;

[GlobalClass]
public partial class TestData : Node
{
    [Export]
    public int initial_random_relics = 0;

    [Export]
    public Godot.Collections.Array<string> initial_relics_ids = new();

    [Export]
    public Godot.Collections.Array<string> initial_consumables_ids = new();

    [Export]
    public Variant initial_event;

    [Export]
    public bool open_loot_screen = false;

    private RunHandler _runHandler;

    public async override void _Ready()
    {
        this._runHandler = GetNodeOrNull<RunHandler>("../RunHandler");

        await ToSignal(GetTree().CreateTimer(0.1f, false), SceneTreeTimer.SignalName.Timeout);
        this._handle_initial_relics();

        if (this.open_loot_screen && this._runHandler != null)
        {
            await this._runHandler.ShowLootScreen();
        }

        GodotObject eventData = this.initial_event.AsGodotObject();
        if (eventData != null && this._runHandler != null)
        {
            await this._runHandler.ShowEventsScreen(eventData);
        }
    }

    private void _handle_initial_relics()
    {
        DataLoader dataLoader = GetNode<DataLoader>("/root/DataLoader");
        RunContext runContext = GetNode<RunContext>("/root/RunContext");

        if (this.initial_relics_ids.Count > 0)
        {
            for (int index = 0; index < this.initial_relics_ids.Count; index++)
            {
                string relicId = this.initial_relics_ids[index];
                Variant relicData = dataLoader.get_relic_by_id(relicId);
                GodotObject relicDataObj = relicData.AsGodotObject();
                if (relicDataObj != null)
                {
                    Variant relicInstance = relicDataObj.Call("create_item");
                    runContext.relics_manager.add_relic(relicInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial relic id {relicId} not found");
                }
            }
        }

        if (this.initial_random_relics > 0)
        {
            Godot.Collections.Array<Variant> items = dataLoader.get_random_relics(this.initial_random_relics);
            for (int index = 0; index < items.Count; index++)
            {
                GodotObject itemObj = items[index].AsGodotObject();
                if (itemObj == null)
                {
                    continue;
                }

                Variant relic = itemObj.Call("create_item");
                runContext.relics_manager.add_relic(relic);
            }
        }

        if (this.initial_consumables_ids.Count > 0)
        {
            for (int index = 0; index < this.initial_consumables_ids.Count; index++)
            {
                string consumableId = this.initial_consumables_ids[index];
                Variant consumableData = dataLoader.get_consumable_by_id(consumableId);
                GodotObject consumableDataObj = consumableData.AsGodotObject();
                if (consumableDataObj != null)
                {
                    Variant consumableInstance = consumableDataObj.Call("create_item");
                    runContext.consumables_manager.add_consumable(consumableInstance);
                }
                else
                {
                    GD.PushError($"[Game]: initial consumable id {consumableId} not found");
                }
            }
        }
    }
}
