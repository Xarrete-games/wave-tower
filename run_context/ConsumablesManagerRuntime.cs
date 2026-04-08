using System.Collections.Generic;

public sealed class ConsumablesManagerRuntime
{
    private const int _maxConsumables = 5;
    private readonly List<ConsumableModel> _consumables = new();

    public IReadOnlyList<ConsumableModel> GetConsumables()
    {
        return this._consumables;
    }

    public bool IsFull()
    {
        return this._consumables.Count >= _maxConsumables;
    }

    public bool AddConsumable(ConsumableModel consumable)
    {
        if (consumable == null || this.IsFull())
        {
            return false;
        }

        this._consumables.Add(consumable);
        return true;
    }

    public bool RemoveConsumable(ConsumableModel consumable)
    {
        if (consumable == null)
        {
            return false;
        }

        return this._consumables.Remove(consumable);
    }

    public void Reset()
    {
        this._consumables.Clear();
    }
}
