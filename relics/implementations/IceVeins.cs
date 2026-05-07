public sealed class IceVeins : Relic
{
    public IceVeins() : base("ice_veins")
    {
    }

    public override void OnDebuffApplied(DebuffContext context, Enemy target)
    {
        if (context.Debuff.Type == EnemyDebuffModel.DebuffType.Frost)
        {
            context.Stacks += 1;
        }
    }
}
