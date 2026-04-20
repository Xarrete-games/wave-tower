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
    public CanvasLayer EventLayer;

    [Export]
    public EventsScreenHandler EventsScreenHander;

    [Export]
    public LootScreenHandler LootScreenHandler;

    private List<EventData> _events = new();
    private EventData _shopEvent;
    private EventData _chooseRelicEvent;
    private List<EventData> _optionsEvents = new();
    private RunProgress _progress;

    public override void _Ready()
    {
        SetEventsByType();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        _progress = runContext.Progress;
        _progress.CurrentWaveFinished += OnWaveFinished;
        _progress.LastWaveFinished += OnLastWaveFinished;

        ShowNextWaveScreen();
    }

    public override void _ExitTree()
    {
        if (_progress != null)
        {
            _progress.CurrentWaveFinished -= OnWaveFinished;
            _progress.LastWaveFinished -= OnLastWaveFinished;
        }
    }

    public async Task ShowLootScreen()
    {
        if (LootScreenHandler == null)
        {
            GD.PushError("[RunHandler] LootScreenHandler is null.");
            return;
        }

        await LootScreenHandler.ShowLootScreenAsync(EventLayer);
    }

    public async Task ShowChooseCardScreen()
    {
        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        var cards = runContext.TowersManager.GetRandomTowers(3);

        ChooseTowerScreen chooseTowerScreen = ChooseTowerScreenScene.Instantiate<ChooseTowerScreen>();
        EventLayer.AddChild(chooseTowerScreen);
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
        if (EventsScreenHander == null || eventData == null)
        {
            return;
        }

        await EventsScreenHander.ShowEventSelectedAsync(eventData, EventLayer);
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
        EventLayer.CallDeferred(Node.MethodName.AddChild, nextWaveScreen);
    }

    private void ShowNextLevelMenu()
    {
        Node nextLevelScreen = NextLevelScreenScene.Instantiate();
        EventLayer.CallDeferred(Node.MethodName.AddChild, nextLevelScreen);
    }

    private async void OnWaveFinished()
    {
        GetNode<AudioManager>("/root/AudioManager").PlayWaveClear();
        await ShowLootScreen();
        await ShowChooseCardScreen();

        RunContext runContext = GetNode<RunContext>("/root/RunContext");
        if (runContext.IsOnRestarting)
        {
            return;
        }

        int currentWave = runContext.Progress.CurrentWave;
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
        runContext.Economy.Gold += 50;

        bool isLastWave = runContext.Progress.IsLastWave();
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
