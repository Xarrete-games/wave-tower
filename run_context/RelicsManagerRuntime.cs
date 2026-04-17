using System.Collections.Generic;

public sealed class RelicsManagerRuntime
{
    private readonly List<Relic> _relicListeners = new();
    private readonly Dictionary<string, Relic> _relicsById = new();
    private readonly Dictionary<string, int> _relicsCount = new();

    public IReadOnlyList<Relic> GetAllRelicListeners()
    {
        return _relicListeners;
    }

    public IReadOnlyCollection<Relic> GetAllRelics()
    {
        return _relicsById.Values;
    }

    public void AddRelic(Relic relic)
    {
        if (relic == null)
        {
            return;
        }

        if (_relicsById.ContainsKey(relic.Id))
        {
            return;
        }

        _relicsById[relic.Id] = relic;
        _relicsCount[relic.Id] = GetCountOrZero(relic.Id) + 1;
        relic.OnObtain();

        if (!_relicListeners.Contains(relic))
        {
            _relicListeners.Add(relic);
        }
    }

    public bool AddRelicById(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return false;
        }

        if (_relicsById.ContainsKey(relicId))
        {
            return false;
        }

        Relic relic = RelicModelFactory.CreateById(relicId);
        if (relic == null)
        {
            return false;
        }

        AddRelic(relic);
        return true;
    }

    public void RemoveRelic(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return;
        }

        if (!_relicsById.TryGetValue(relicId, out Relic relic))
        {
            return;
        }

        _relicsById.Remove(relicId);
        _relicsCount[relicId] = GetCountOrZero(relicId) - 1;
        relic.OnRemove();
        _relicListeners.Remove(relic);
    }

    public bool HasRelic(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return false;
        }

        if (!_relicsById.TryGetValue(relicId, out Relic relic))
        {
            return false;
        }

        return !relic.Disabled;
    }

    public Relic GetRelic(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return null;
        }

        if (!_relicsById.TryGetValue(relicId, out Relic relic))
        {
            return null;
        }

        return relic;
    }

    public int GetRelicCount(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return 0;
        }

        return GetCountOrZero(relicId);
    }

    public float ApplyPriceHookForRelic(string relicId, int priceType, int basePrice, float currentDiscount)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return currentDiscount;
        }

        if (!_relicsById.TryGetValue(relicId, out Relic relic))
        {
            return currentDiscount;
        }

        if (relic.Disabled)
        {
            return currentDiscount;
        }

        var context = new PriceContext((PriceContext.PriceType)priceType, basePrice)
        {
            Discount = currentDiscount,
        };

        relic.OnGetPrice(context);
        return context.Discount;
    }

    private int GetCountOrZero(string relicId)
    {
        if (!_relicsCount.TryGetValue(relicId, out int count))
        {
            return 0;
        }

        return count;
    }

    public void Reset()
    {
        _relicListeners.Clear();
        _relicsById.Clear();
        _relicsCount.Clear();
    }
}
