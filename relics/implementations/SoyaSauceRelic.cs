using Godot;

public partial class SoyaSauceRelic : RelicRuntimeAdapter
{
    private readonly SoyaSauce _model = new SoyaSauce();

    protected override RelicModel Model => this._model;

    public override void on_get_price(Variant context)
    {
        this._model.HasTunaNigiri = this.HasRelic("tuna_nigiri");
        this._model.HasSalmonNigiri = this.HasRelic("salmon_nigiri");
        this._model.HasButterfishNigiri = this.HasRelic("butterfish_nigiri");
        base.on_get_price(context);
    }

    private bool HasRelic(string relicId)
    {
        SceneTree tree = Engine.GetMainLoop() as SceneTree;
        if (tree == null)
        {
            return false;
        }

        Node runContext = tree.Root.GetNodeOrNull<Node>("/root/RunContext");
        if (runContext == null)
        {
            return false;
        }

        Variant relicsManagerVariant = runContext.Get("relics_manager");
        if (relicsManagerVariant.VariantType == Variant.Type.Nil)
        {
            return false;
        }

        GodotObject relicsManager = relicsManagerVariant.AsGodotObject();
        if (relicsManager == null)
        {
            return false;
        }

        return (bool)relicsManager.Call("has_relic", relicId);
    }
}
