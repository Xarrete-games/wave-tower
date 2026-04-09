using Godot;

[GlobalClass]
public partial class CaptainCapRelic : RelicRuntimeAdapter
{
    private readonly CaptainCap _model = new CaptainCap();

    protected override RelicModel Model => this._model;
}
