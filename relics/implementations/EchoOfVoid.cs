public sealed class EchoOfVoid : TowerBuffRelicBase
{
    public EchoOfVoid() : base("echo_of_void")
    {
    }

    protected override void AddBuff(TowerModel tower)
    {
        tower.CurrentBounces += 1;
    }

    protected override void RemoveBuff(TowerModel tower)
    {
        tower.CurrentBounces -= 1;
    }

    protected override bool IsValidTower(TowerModel tower)
    {
        return tower.TypeId == "lightning_chain_tower";
    }
}
