public sealed class PhoenixFeather : RelicModel
{
    public PhoenixFeather() : base("phoenix_feather")
    {
    }

    public override void OnBeforeDie(StatusModel status)
    {
        if (this.Disabled)
        {
            return;
        }

        status.Heal(10);
        this.Disabled = true;
    }
}
