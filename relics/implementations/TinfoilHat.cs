public sealed class TinfoilHat : RelicModel
{
    public TinfoilHat() : base("tinfoil_hat")
    {
    }

    public override void OnRelicAdded(RelicModel relicAdded)
    {
        if (this.Disabled || !relicAdded.IsCursed)
        {
            return;
        }

        RunContextRuntime.RelicsManager.RemoveRelic(relicAdded.Id);
        this.Disabled = true;
    }
}
