public sealed class StatusModel
{
    public int MaxHealth { get; set; } = 20;
    public int Health { get; set; } = 20;
    public int Armor { get; set; }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Health = System.Math.Min(Health + amount, MaxHealth);
    }

    public void AddArmor(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Armor += amount;
    }

    public void AddMaxHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        MaxHealth += amount;
        Health = System.Math.Min(Health + amount, MaxHealth);
    }

    public void ChangeMaxHealth(int amount)
    {
        if (amount == 0)
        {
            return;
        }

        MaxHealth += amount;
        if (MaxHealth < 1)
        {
            MaxHealth = 1;
        }

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }
}
