using System;

public abstract class Relic : AbstractModel
{
    public event Action<Relic> Changed;

    public RelicData Data { get; private set; }

    public string Id => this.Data?.id ?? this._fallbackId;
    public bool IsCursed { get; }
    private readonly string _fallbackId;

    private bool _disabled;
    public bool Disabled
    {
        get => this._disabled;
        set
        {
            this._disabled = value;
            this.Changed?.Invoke(this);
        }
    }

    private int _counter;
    public int Counter
    {
        get => this._counter;
        set
        {
            this._counter = value;
            this.Changed?.Invoke(this);
        }
    }

    protected Relic(string id, bool isCursed = false)
    {
        this._fallbackId = id;
        this.IsCursed = isCursed;
    }

    public void SetupData(RelicData data)
    {
        this.Data = data;
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
