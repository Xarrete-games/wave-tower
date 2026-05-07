public sealed class PhoenixFeather : Relic
{
    public PhoenixFeather() : base("phoenix_feather")
    {
    }

    public override void OnBeforeDie(StatusModel status)
    {
        if (Disabled)
        {
            return;
        }

        status.Heal(10);
        Disabled = true;
    }
}
