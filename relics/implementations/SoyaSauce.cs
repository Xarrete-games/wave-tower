public sealed class SoyaSauce : RelicModel
{
    private const bool DEBUG_SOYA_SAUCE = true;

    public SoyaSauce() : base("soya_sauce")
    {
    }

    public override void OnGetPrice(PriceContext context)
    {
        float discountBefore = context.Discount;
        bool hasTuna = this.HasRelicById("tuna_nigiri");
        bool hasSalmon = this.HasRelicById("salmon_nigiri");
        bool hasButterfish = this.HasRelicById("butterfish_nigiri");

        switch (context.Type)
        {
            case PriceContext.PriceType.Tower:
                if (hasTuna)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Relic:
                if (hasSalmon)
                {
                    context.Discount += 0.1f;
                }
                break;
            case PriceContext.PriceType.Consumable:
                if (hasButterfish)
                {
                    context.Discount += 0.1f;
                }
                break;
        }

        if (DEBUG_SOYA_SAUCE)
        {
            Godot.GD.Print($"[SoyaSauce] type={context.Type} base={context.BasePrice} discount {discountBefore:0.###}->{context.Discount:0.###} tuna={hasTuna} salmon={hasSalmon} butterfish={hasButterfish}");
        }
    }

    private bool HasRelicById(string relicId)
    {
        var tree = Godot.Engine.GetMainLoop() as Godot.SceneTree;
        var runContext = tree?.Root.GetNodeOrNull<Godot.Node>("/root/RunContext");
        var relicsManager = runContext?.Get("relics_manager").AsGodotObject();
        if (relicsManager != null)
        {
            return relicsManager.Call("has_relic", relicId).AsBool();
        }

        return RunContextRuntime.RelicsManager.HasRelic(relicId);
    }
}
