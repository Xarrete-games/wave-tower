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

        int nextHealth = this.Health + amount;
        this.Health = System.Math.Min(nextHealth, this.MaxHealth);
    }

    public void AddArmor(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        this.Armor += amount;
    }

    public void AddMaxHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        this.MaxHealth += amount;
        this.Health = System.Math.Min(this.Health + amount, this.MaxHealth);
    }

    public void ChangeMaxHealth(int amount)
    {
        if (amount == 0)
        {
            return;
        }

        this.MaxHealth += amount;
        if (this.MaxHealth < 1)
        {
            this.MaxHealth = 1;
        }

        if (this.Health > this.MaxHealth)
        {
            this.Health = this.MaxHealth;
        }
    }

    public void ApplyDamage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int remainingDamage = amount;

        if (this.Armor > 0)
        {
            int absorbed = System.Math.Min(this.Armor, remainingDamage);
            this.Armor -= absorbed;
            remainingDamage -= absorbed;
        }

        if (remainingDamage > 0)
        {
            this.Health -= remainingDamage;
        }
    }

    public void SyncFromLegacy(int maxHealth, int health, int armor)
    {
        this.MaxHealth = maxHealth;
        this.Health = health;
        this.Armor = armor;
    }
}
