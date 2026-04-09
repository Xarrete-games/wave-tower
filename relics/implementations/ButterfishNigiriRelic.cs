using Godot;

[GlobalClass]
public partial class ButterfishNigiriRelic : RelicRuntimeAdapter
{
    private readonly ButterfishNigiri _model = new ButterfishNigiri();

    protected override RelicModel Model => this._model;
}
