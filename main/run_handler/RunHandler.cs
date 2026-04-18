using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class RunHandler : Node
{
    private static readonly PackedScene NextWaveScreenScene = GD.Load<PackedScene>("uid://b7ttkk4pasgin");
    private static readonly PackedScene NextLevelScreenScene = GD.Load<PackedScene>("uid://crastw7xnqgvl");
    private static readonly PackedScene EndGameScene = GD.Load<PackedScene>("uid://ovtc0l4cimpl");
    private static readonly PackedScene ChooseTowerScreenScene = GD.Load<PackedScene>("uid://bxw2isn60dlre");

    private static readonly int[] WavesWithEvents = { 8 };
    private static readonly int[] WavesWithShops = { 4 };
    private static readonly int[] WavesWithRelics = { 2, 6, 10 };

    [Export]
    public CanvasLayer event_layer;

    [Export]
    public EventsScreenHandler events_screen_hander;

    [Export]
    public LootScreenHandler loot_screen_handler;

    private List<EventData> _events = new();
    private EventData _shopEvent;
    private EventData _chooseRelicEvent;
    private List<EventData> _optionsEvents = new();
    private RunProgress _progress;

    public override void _Ready()
    {
        SetEventsByType();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext.progress;
        _progress.current_wave_finished += OnWaveFinished;
        _progress.last_wave_finished += OnLastWaveFinished;

        ShowNextWaveScreen();
    }

    public override void _ExitTree()
    {
        if (_progress != null)
        {
            _progress.current_wave_finished -= OnWaveFinished;
            _progress.last_wave_finished -= OnLastWaveFinished;
        }
    }

    public async Task ShowLootScreen()
    {
        if (loot_screen_handler == null)
        {
            GD.PushError("[RunHandler] loot_screen_handler is null.");
            return;
        }

        await loot_screen_handler.ShowLootScreenAsync(event_layer);
    }

    public async Task ShowChooseCardScreen()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        var cards = runContext.towers_manager.get_random_towers(3);

        ChooseTowerScreen chooseTowerScreen = ChooseTowerScreenScene.Instantiate<ChooseTowerScreen>();
        event_layer.AddChild(chooseTowerScreen);
        chooseTowerScreen.PopulateScreen(cards);

        var completion = new TaskCompletionSource<bool>();
        void OnDone()
        {
            completion.TrySetResult(true);
        }

        chooseTowerScreen.done += OnDone;
        try
        {
            await completion.Task;
        }
        finally
        {
            if (GodotObject.IsInstanceValid(chooseTowerScreen))
            {
                chooseTowerScreen.done -= OnDone;
            }
        }
    }

    public async Task ShowEventsScreen(EventData eventData)
    {
        if (events_screen_hander == null || eventData == null)
        {
            return;
        }

        await events_screen_hander.ShowEventSelectedAsync(eventData, event_layer);
    }

    private void SetEventsByType()
    {
        _events = DataLoaderAccess.GetAllEventsTyped();
        _shopEvent = null;
        _chooseRelicEvent = null;
        _optionsEvents = new List<EventData>();

        foreach (EventData eventData in _events)
        {
            if (eventData == null)
            {
                continue;
            }

            int eventType = (int)eventData.Type;
            if (eventType == 1)
            {
                _shopEvent = eventData;
            }
            else if (eventType == 2)
            {
                _chooseRelicEvent = eventData;
            }
            else if (eventType == 0)
            {
                _optionsEvents.Add(eventData);
            }
        }
    }

    private void ShowNextWaveScreen()
    {
        Node nextWaveScreen = NextWaveScreenScene.Instantiate();
        event_layer.CallDeferred(Node.MethodName.AddChild, nextWaveScreen);
    }

    private void ShowNextLevelMenu()
    {
        Node nextLevelScreen = NextLevelScreenScene.Instantiate();
        event_layer.CallDeferred(Node.MethodName.AddChild, nextLevelScreen);
    }

    private async void OnWaveFinished()
    {
        GetNode<AudioManager>("/root/AudioManager").play_wave_clear();
        await ShowLootScreen();
        await ShowChooseCardScreen();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (runContext.is_on_restarting)
        {
            return;
        }

        int currentWave = runContext.progress.current_wave;
        EventData eventData = GetNextEvent(currentWave);
        if (eventData == null)
        {
            ShowNextWaveScreen();
            return;
        }

        await ShowEventsScreen(eventData);
        ShowNextWaveScreen();
    }

    private async void OnLastWaveFinished()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        runContext.economy.gold += 50;

        bool isLastWave = runContext.progress.is_last_wave();
        if (isLastWave)
        {
            await ToSignal(GetTree().CreateTimer(5, false), SceneTreeTimer.SignalName.Timeout);
            GetTree().ChangeSceneToPacked(EndGameScene);
            return;
        }

        ShowNextLevelMenu();
    }

    private EventData GetNextEvent(int currentWave)
    {
        int waveInCycle = ((currentWave - 1) % 10) + 1;

        if (Contains(WavesWithShops, waveInCycle) && _shopEvent != null)
        {
            return _shopEvent;
        }

        if (Contains(WavesWithRelics, waveInCycle) && _chooseRelicEvent != null)
        {
            return _chooseRelicEvent;
        }

        if (Contains(WavesWithEvents, waveInCycle) && _optionsEvents.Count > 0)
        {
            int randomIndex = (int)(GD.Randi() % (uint)_optionsEvents.Count);
            EventData picked = _optionsEvents[randomIndex];
            _optionsEvents.RemoveAt(randomIndex);
            return picked;
        }

        return null;
    }

    private static bool Contains(int[] source, int value)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] == value)
            {
                return true;
            }
        }

        return false;
    }
}
