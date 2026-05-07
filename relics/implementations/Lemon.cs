public sealed class Lemon : Relic
{
    private bool _isActive = true;

    public Lemon() : base("lemon", true)
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        _isActive = false;
    }

    public override void OnGetPrice(PriceContext context)
    {
        if (context.Type == PriceContext.PriceType.Tower && _isActive)
        {
            context.Discount += 0.5f;
        }
    }

    public override void OnWaveFinished()
    {
        _isActive = true;
    }
}
