public sealed class CrownOfTheForgottenKing : RelicModel
{
    public CrownOfTheForgottenKing() : base("crown_of_the_forgotten_king")
    {
    }

    public override void OnWaveFinished()
    {
        RunContextRuntime.CompositeTileMap.DestroyRandomBuildableTile();
    }
}
