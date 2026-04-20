public class DamageFlatBuff : TowerBuffStatsModifier {
    public DamageFlatBuff() {
    }
    public DamageFlatBuff(Source source, Duration duration = null, TowerBuff residualBuff = null, BuffData buffData = null, int value = 0) : base(source, duration, residualBuff, buffData, value) {
    }
    public override void Contribute(TowerStatsAccumulator acc) {
        acc.FlatDamage += Value;
    }
}

