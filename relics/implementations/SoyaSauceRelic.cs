using Godot;

[GlobalClass]
public partial class SoyaSauceRelic : RelicRuntimeAdapter
{
    private readonly SoyaSauce _model = new SoyaSauce();

    protected override RelicModel Model => this._model;

    public override void on_get_price(Variant context)
    {
        this._model.HasTunaNigiri = this.HasRelicById("tuna_nigiri");
        this._model.HasSalmonNigiri = this.HasRelicById("salmon_nigiri");
        this._model.HasButterfishNigiri = this.HasRelicById("butterfish_nigiri");
        base.on_get_price(context);
    }
}
