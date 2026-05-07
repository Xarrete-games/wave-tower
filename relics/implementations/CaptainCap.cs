public sealed class CaptainCap : Relic
{
    public CaptainCap() : base("captain_cap")
    {
    }

    public override void OnBeforeRelicReward(RelicsRewardsContext context)
    {
        context.NumberOfRelics += 1;
    }
}
