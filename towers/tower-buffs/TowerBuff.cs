public class TowerBuff {
    public Source source;
    public BuffData data;
    public Duration duration;
    public TowerBuff residual_buff;
    public TowerBuff() {
    }
    public TowerBuff(Source buffSource, Duration buffDuration = null, TowerBuff residualBuff = null, BuffData buffData = null) {
        source = buffSource;
        duration = buffDuration;
        residual_buff = residualBuff;
        data = buffData;
    }
}

