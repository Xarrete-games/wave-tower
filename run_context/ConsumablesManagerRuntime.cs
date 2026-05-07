using System.Collections.Generic;

public sealed class ConsumablesManagerRuntime
{
    private const int _maxConsumables = 5;
    private readonly List<ConsumableModel> _consumables = new();

    public IReadOnlyList<ConsumableModel> GetConsumables()
    {
        return _consumables;
    }

    public bool IsFull()
    {
        return _consumables.Count >= _maxConsumables;
    }

    public bool AddConsumable(ConsumableModel consumable)
    {
        if (consumable == null || IsFull())
        {
            return false;
        }

        _consumables.Add(consumable);
        return true;
    }

    public bool RemoveConsumable(ConsumableModel consumable)
    {
        if (consumable == null)
        {
            return false;
        }

        return _consumables.Remove(consumable);
    }

    public void Reset()
    {
        _consumables.Clear();
    }
}
