public sealed class Buda : Relic
{
    public Buda() : base("buda")
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        RunContextRuntime.Status.ChangeMaxHealth(1);
    }
}
