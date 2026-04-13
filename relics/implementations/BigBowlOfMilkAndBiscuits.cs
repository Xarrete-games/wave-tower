public sealed class BigBowlOfMilkAndBiscuits : Relic
{
    private const int _maxHealthBonus = 10;

    public BigBowlOfMilkAndBiscuits() : base("big_bowl_of_milk_and_biscuits")
    {
    }

    public override void OnObtain()
    {
        RunContextRuntime.Status.AddMaxHealth(_maxHealthBonus);
        RunContextRuntime.Status.Heal(RunContextRuntime.Status.MaxHealth);
    }
}
