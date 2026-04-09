public partial class StrategyTomeHighHealthPriorityRelic : RelicRuntimeAdapter
{
    private readonly StrategyTomeHighHealthPriority _model = new StrategyTomeHighHealthPriority();

    protected override RelicModel Model => this._model;
}
