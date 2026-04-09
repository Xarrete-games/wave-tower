using Godot;

[GlobalClass]
public partial class IceCreamRelic : RelicRuntimeAdapter
{
    private readonly IceCream _model = new IceCream();

    protected override RelicModel Model => this._model;
}
