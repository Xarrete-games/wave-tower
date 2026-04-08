public abstract class RelicModel : AbstractModel
{
    public string Id { get; }
    public bool IsCursed { get; }
    public bool Disabled { get; set; }
    public int Counter { get; set; }

    protected RelicModel(string id, bool isCursed = false)
    {
        this.Id = id;
        this.IsCursed = isCursed;
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
