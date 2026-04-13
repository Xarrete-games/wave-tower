public sealed class CrownOfTheForgottenKing : Relic
{
    public CrownOfTheForgottenKing() : base("crown_of_the_forgotten_king")
    {
    }

    public override void OnWaveFinished()
    {
        RunContextRuntime.CompositeTileMap.DestroyRandomBuildableTile();
    }
}
