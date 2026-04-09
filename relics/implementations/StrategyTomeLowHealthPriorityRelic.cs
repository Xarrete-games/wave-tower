public partial class StrategyTomeLowHealthPriorityRelic : RelicRuntimeAdapter
{
    private readonly StrategyTomeLowHealthPriority _model = new StrategyTomeLowHealthPriority();

    protected override RelicModel Model => this._model;
}
