public partial class SalmonNigiriRelic : RelicRuntimeAdapter
{
    private readonly SalmonNigiri _model = new SalmonNigiri();

    protected override RelicModel Model => this._model;
}
