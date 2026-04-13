public sealed class PowerGloves : Relic
{
    private const string _buffId = "damage_flat_buff";
    private const int _buffValue = 3;

    public PowerGloves() : base("power_gloves")
    {
    }

    public override void OnConsumableUsed(ConsumableModel consumable)
    {
        if (consumable is not ConsumableTargeteableModel targeteable)
        {
            return;
        }

        if (targeteable.TargetingType != ConsumableTargeteableModel.TargetType.Tower)
        {
            return;
        }

        if (targeteable.TargetTower == null)
        {
            return;
        }

        TowerBuffModel buff = TowerBuffFactoryModel.CreateFromId(_buffId, this.Id, _buffValue);
        if (buff != null)
        {
            targeteable.TargetTower.AddBuff(buff);
        }
    }
}
