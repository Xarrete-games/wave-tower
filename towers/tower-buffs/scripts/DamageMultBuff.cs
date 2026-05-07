public class DamageMultBuff : TowerBuffStatsModifier {
    public DamageMultBuff() {
    }
    public DamageMultBuff(Source source, Duration duration = null, TowerBuff residualBuff = null, BuffData buffData = null, int value = 0) : base(source, duration, residualBuff, buffData, value) {
    }
    public override void Contribute(TowerStatsAccumulator acc) {
        acc.DamageMult += Value / 100.0f;
    }
}

