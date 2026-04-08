public sealed class SafetyHelmet : RelicModel
{
    public SafetyHelmet() : base("safety_helmet")
    {
    }

    public override void OnWaveInit()
    {
        RunContextRuntime.Status.AddArmor(1);
    }
}
