using Godot;

[GlobalClass]
public partial class PhoenixFeatherRelic : RelicRuntimeAdapter
{
    private const int HEAL_AMOUNT = 10;

    private readonly PhoenixFeather _model = new();

    protected override RelicModel Model => this._model;

    public override void on_before_die(Variant status)
    {
        if (this.disabled)
        {
            return;
        }

        GodotObject statusObj = status.AsGodotObject();
        if (statusObj == null)
        {
            return;
        }

        statusObj.Call("heal", HEAL_AMOUNT);
        this.disabled = true;
    }
}
