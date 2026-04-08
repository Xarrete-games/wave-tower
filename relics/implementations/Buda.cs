public sealed class Buda : RelicModel
{
    public Buda() : base("buda")
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        RunContextRuntime.Status.ChangeMaxHealth(1);
    }
}
