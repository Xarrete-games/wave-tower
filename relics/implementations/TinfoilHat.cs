public sealed class TinfoilHat : Relic
{
    public TinfoilHat() : base("tinfoil_hat")
    {
    }

    public override void OnRelicAdded(Relic relicAdded)
    {
        if (Disabled || !relicAdded.IsCursed)
        {
            return;
        }

        RunContextRuntime.RelicsManager.RemoveRelic(relicAdded.Id);
        Disabled = true;
    }
}
