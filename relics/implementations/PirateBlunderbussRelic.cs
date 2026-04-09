using Godot;

[GlobalClass]
public partial class PirateBlunderbussRelic : RelicRuntimeAdapter
{
    private readonly PirateBlunderbuss _model = new PirateBlunderbuss();

    protected override RelicModel Model => this._model;
}
