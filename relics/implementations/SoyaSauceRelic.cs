using Godot;

[GlobalClass]
public partial class SoyaSauceRelic : RelicRuntimeAdapter
{
    private readonly SoyaSauce _model = new SoyaSauce();

    protected override RelicModel Model => this._model;
}
