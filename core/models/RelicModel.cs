using System;

public abstract class Relic : AbstractModel
{
    public event Action<Relic> Changed;

    public RelicData Data { get; private set; }

    public string Id => Data?.Id ?? _fallbackId;
    public bool IsCursed { get; }
    private readonly string _fallbackId;

    private bool _disabled;
    public bool Disabled
    {
        get => _disabled;
        set
        {
            _disabled = value;
            Changed?.Invoke(this);
        }
    }

    private int _counter;
    public int Counter
    {
        get => _counter;
        set
        {
            _counter = value;
            Changed?.Invoke(this);
        }
    }

    protected Relic(string id, bool isCursed = false)
    {
        _fallbackId = id;
        IsCursed = isCursed;
    }

    public void SetupData(RelicData data)
    {
        Data = data;
    }

    public virtual void OnObtain()
    {
    }

    public virtual void OnRemove()
    {
    }

    protected bool HasRelic(string relicId)
    {
        return RunContextRuntime.RelicsManager.HasRelic(relicId);
    }

    protected int GetRelicCount(string relicId)
    {
        return RunContextRuntime.RelicsManager.GetRelicCount(relicId);
    }
}
