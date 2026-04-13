using System.Collections.Generic;

public sealed class RelicsManagerRuntime
{
    private readonly List<Relic> _relicListeners = new();
    private readonly Dictionary<string, Relic> _relicsById = new();
    private readonly Dictionary<string, int> _relicsCount = new();

    public IReadOnlyList<Relic> GetAllRelicListeners()
    {
        return this._relicListeners;
    }

    public IReadOnlyCollection<Relic> GetAllRelics()
    {
        return this._relicsById.Values;
    }

    public void AddRelic(Relic relic)
    {
        if (relic == null)
        {
            return;
        }

        if (this._relicsById.ContainsKey(relic.Id))
        {
            return;
        }

        this._relicsById[relic.Id] = relic;
        this._relicsCount[relic.Id] = this.GetCountOrZero(relic.Id) + 1;
        relic.OnObtain();

        if (!this._relicListeners.Contains(relic))
        {
            this._relicListeners.Add(relic);
        }
    }

    public bool AddRelicById(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return false;
        }

        if (this._relicsById.ContainsKey(relicId))
        {
            return false;
        }

        Relic relic = RelicModelFactory.CreateById(relicId);
        if (relic == null)
        {
            return false;
        }

        this.AddRelic(relic);
        return true;
    }

    public void RemoveRelic(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return;
        }

        if (!this._relicsById.TryGetValue(relicId, out Relic relic))
        {
            return;
        }

        this._relicsById.Remove(relicId);
        this._relicsCount[relicId] = this.GetCountOrZero(relicId) - 1;
        relic.OnRemove();
        this._relicListeners.Remove(relic);
    }

    public bool HasRelic(string relicId)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return false;
        }

        if (!this._relicsById.TryGetValue(relicId, out Relic relic))
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

        if (!this._relicsById.TryGetValue(relicId, out Relic relic))
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

        return this.GetCountOrZero(relicId);
    }

    public float ApplyPriceHookForRelic(string relicId, int priceType, int basePrice, float currentDiscount)
    {
        if (string.IsNullOrEmpty(relicId))
        {
            return currentDiscount;
        }

        if (!this._relicsById.TryGetValue(relicId, out Relic relic))
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
        if (!this._relicsCount.TryGetValue(relicId, out int count))
        {
            return 0;
        }

        return count;
    }

    public void Reset()
    {
        this._relicListeners.Clear();
        this._relicsById.Clear();
        this._relicsCount.Clear();
    }
}
