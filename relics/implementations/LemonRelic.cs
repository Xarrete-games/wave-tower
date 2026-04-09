public partial class LemonRelic : RelicRuntimeAdapter
{
    private readonly Lemon _model = new Lemon();

    protected override RelicModel Model => this._model;
}
