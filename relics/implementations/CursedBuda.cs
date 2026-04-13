public sealed class CursedBuda : Relic
{
    public CursedBuda() : base("cursed_buda", true)
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        RunContextRuntime.Status.ChangeMaxHealth(-1);
    }
}
