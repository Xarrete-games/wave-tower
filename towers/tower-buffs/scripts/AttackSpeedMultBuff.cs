public class AttackSpeedMultBuff : TowerBuffStatsModifier {
    public AttackSpeedMultBuff() {
    }
    public AttackSpeedMultBuff(Source source, Duration duration = null, TowerBuff residualBuff = null, BuffData buffData = null, int value = 0) : base(source, duration, residualBuff, buffData, value) {
    }
    public override void Contribute(TowerStatsAccumulator acc) {
        acc.AttackSpeedMult += Value / 100.0f;
    }
}

