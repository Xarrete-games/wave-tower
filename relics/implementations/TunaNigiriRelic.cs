public partial class TunaNigiriRelic : RelicRuntimeAdapter
{
    private readonly TunaNigiri _model = new TunaNigiri();

    protected override RelicModel Model => this._model;
}
