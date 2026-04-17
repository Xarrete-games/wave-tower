public sealed class Metronome : Relic
{
    private const string _buffId = "attack_speed_mult_buff";
    private const int _buffValue = 5;
    private const int _counterThreshold = 3;

    public Metronome() : base("metronome")
    {
    }

    public override void OnWaveFinished()
    {
        Counter += 1;
        if (Counter < _counterThreshold)
        {
            return;
        }

        Counter = 0;
        TowerModel tower = RunContextRuntime.TowersManager.PickRandomTower();
        if (tower == null)
        {
            return;
        }

        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, Id, _buffValue);
        if (buff != null)
        {
            tower.AddBuff(buff);
        }
    }
}
