public sealed class CaptainCap : RelicModel
{
    public CaptainCap() : base("captain_cap")
    {
    }

    public override void OnBeforeRelicReward(RelicsRewardsContext context)
    {
        context.NumberOfRelics += 1;
    }
}
