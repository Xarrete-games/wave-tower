public sealed class PaganiniBow : Relic
{
    private const string _buffId = "damage_mult_buff";
    private const int _buffValue = 10;
    private const int _counterThreshold = 4;

    public PaganiniBow() : base("paganini_bow")
    {
    }

    public override void OnTowerPlaced(TowerModel tower)
    {
        Counter += 1;
        if (Counter < _counterThreshold)
        {
            return;
        }

        Counter = 0;
        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, Id, _buffValue);
        if (buff != null)
        {
            tower.AddBuff(buff);
        }
    }
}
