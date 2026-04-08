public sealed class CursedBuda : RelicModel
{
    public CursedBuda() : base("cursed_buda", true)
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        RunContextRuntime.Status.ChangeMaxHealth(-1);
    }
}
