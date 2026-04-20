public class TowerBuff {
    public Source Source;
    public BuffData Data;
    public Duration Duration;
    public TowerBuff ResidualBuff;
    public TowerBuff() {
    }
    public TowerBuff(Source buffSource, Duration buffDuration = null, TowerBuff residualBuff = null, BuffData buffData = null) {
        Source = buffSource;
        Duration = buffDuration;
        ResidualBuff = residualBuff;
        Data = buffData;
    }
}

