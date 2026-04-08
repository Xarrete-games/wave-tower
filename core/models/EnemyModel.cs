public sealed class EnemyModel
{
    public float MaxHealth { get; set; }
    public float RemainingHealth { get; set; }
    public float ProgressRatio { get; set; }
    public int GoldValue { get; set; }
    public bool HasAnyDebuff { get; set; }

    public float GetPercentageRemainingHealth()
    {
        if (this.MaxHealth <= 0f)
        {
            return 0f;
        }

        float healthRatio = this.RemainingHealth / this.MaxHealth;
        float percentage = healthRatio * 100f;
        return System.MathF.Min(100f, percentage);
    }
}
