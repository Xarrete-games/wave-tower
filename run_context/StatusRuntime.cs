public sealed class StatusRuntime
{
    public int MaxHealth { get; private set; } = 20;
    public int Health { get; private set; } = 20;
    public int Armor { get; private set; }

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int nextHealth = Health + amount;
        Health = System.Math.Min(nextHealth, MaxHealth);
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

    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int remainingDamage = amount;

        if (Armor > 0)
        {
            int absorbed = System.Math.Min(Armor, remainingDamage);
            Armor -= absorbed;
            remainingDamage -= absorbed;
        }

        if (remainingDamage > 0)
        {
            Health -= remainingDamage;
        }
    }

    public void SyncFromLegacy(int maxHealth, int health, int armor)
    {
        MaxHealth = maxHealth;
        Health = health;
        Armor = armor;
    }
}
